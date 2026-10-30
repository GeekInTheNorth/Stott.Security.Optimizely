import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { httpGet } from '../Common/httpClient';
import { Box, Button, Card, CardHeader, DateRangePicker, DateRangePickerContent, DateRangePickerTrigger, Field, Grid, Group, Input, Table, TableBody, TableCell, TableHeader, TableHeaderCell, TableRow, Text } from '@optiaxiom/react';
import { format } from "date-fns";
import FormSelect from '../Common/FormSelect';

const operationTypeOptions = [
    { label: 'All', value: '' },
    { label: 'Added', value: 'Added' },
    { label: 'Deleted', value: 'Deleted' },
    { label: 'Modified', value: 'Modified' }
];

const recordTypeOptions = [
    { label: 'All', value: '' },
    { label: 'CORS Settings', value: 'CORS Settings' },
    { label: 'CSP Settings', value: 'CSP Settings' },
    { label: 'CSP Source', value: 'CSP Source' },
    { label: 'CSP Sandbox', value: 'CSP Sandbox' },
    { label: 'Response Header', value: 'Response Header' },
    { label: 'Security.txt', value: 'security.txt' }
];

const pageSizeOptions = [
    { label: '10', value: 10 },
    { label: '25', value: 25 },
    { label: '100', value: 100 }
];

function AuditHistory(props) {

    const [mounted, setMounted] = useState(false);
    const [auditUsers, setAuditUsers] = useState([]);
    const [auditHistory, setAuditHistory] = useState([]);

    const [startDate, setStartDate] = useState(new Date());
    const [endDate, setEndDate] = useState(new Date());

    const [selectedUser, setSelectedUser] = useState('');
    const [selectedOperationType, setSelectedOperationType] = useState('');
    const [selectedRecordType, setSelectedRecordType] = useState('');
    const [selectedFrom, setSelectedFrom] = useState(0);
    const [selectedPageSize, setSelectedPageSize] = useState(10);
    const [searchTerm, setSearchTerm] = useState('');
    const [debouncedSearchTerm, setDebouncedSearchTerm] = useState('');

    const setMonthStart = () => {
        var today = new Date();
        setStartDate(new Date(today.getFullYear(), today.getMonth(), today.getDate() - 30));
    };

    const handleSelectUser = (value) => {
        setSelectedFrom(0);
        setSelectedUser(value);
    };

    const handleSelectOperationType = (value) => {
        setSelectedFrom(0);
        setSelectedOperationType(value);
    };

    const handleSelectRecordType = (value) => {
        setSelectedFrom(0);
        setSelectedRecordType(value);
    };

    const handleSelectPageSize = (value) => {
        setSelectedFrom(0);
        setSelectedPageSize(Number(value));
    };

    const handleSelectDateRange = (selectedRange) => {
        if (!selectedRange || !selectedRange.from || !selectedRange.to) {
            return;
        }

        setSelectedFrom(0);
        setStartDate(selectedRange.from);
        setEndDate(selectedRange.to);
    };

    const handleSearchTermChange = (event) => {
        setSelectedFrom(0);
        setSearchTerm(event.target.value);
    };

    const handleLoadMore = () => {
        let newSelectedFrom = selectedFrom + selectedPageSize;
        setSelectedFrom(newSelectedFrom);
    };

    const getAuditHistory = async () => {
        await httpGet(import.meta.env.VITE_AUDIT_LIST_URL, {
            dateFrom: startDate,
            dateTo: endDate,
            actionedBy: selectedUser,
            recordType: selectedRecordType,
            operationType: selectedOperationType,
            from: selectedFrom,
            take: selectedPageSize,
            searchTerm: debouncedSearchTerm
        })
        .then((response) => {
            if (selectedFrom === 0){
                setAuditHistory(response.data);
            }
            else {
                let additional = response.data;
                setAuditHistory(current => [...current, ...additional]);
            }
        },
        () => {
            handleShowFailureToast("Error", "Failed to load audit history.");
        });
    };

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const getAuditUsers = async () => {
        await httpGet(import.meta.env.VITE_AUDIT_USER_URL)
            .then((response) => {
                setAuditUsers(response.data);
            },
            () => {
                handleShowFailureToast("Error", "Failed to load users for audit history.");
            });
    };

    const userOptions = [{ label: 'All', value: '' }].concat(auditUsers.map(user => ({ label: user, value: user })));

    const renderAuditHistoryCards = () => {
        return auditHistory && auditHistory.map(auditEntry => {
            const { id, actioned, actionedBy, operationType, recordType, identifier, changes } = auditEntry
            return(
                <Card id={id} my='16' key={id}>
                    <CardHeader description={identifier === '' ? undefined : `${operationType} ${getIdentifierName(recordType)}: ${identifier}`}>
                        <strong>{recordType}</strong> was <strong>{operationType}</strong> by <strong>{actionedBy}</strong> on <strong>{format(new Date(actioned), "dd MMM yyyy 'at' HH:mm:ss")}</strong>
                    </CardHeader>
                    <Table w='full'>
                        <TableHeader>
                            <TableRow>
                                <TableHeaderCell>Field</TableHeaderCell>
                                <TableHeaderCell>Old Value</TableHeaderCell>
                                <TableHeaderCell>New Value</TableHeaderCell>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {renderAuditHistoryCardDetails(changes)}
                        </TableBody>
                    </Table>
                </Card>
            )
        })
    };

    const renderAuditHistoryCardDetails = (auditChanges) => {
        return auditChanges && auditChanges.map(auditChange => {
            const { id, field, oldValue, newValue } = auditChange
            return(
                <TableRow key={id}>
                    <TableCell>{renderAuditName(field)}</TableCell>
                    <TableCell className='allow-word-break'>{renderAuditValue(oldValue)}</TableCell>
                    <TableCell className='allow-word-break'>{renderAuditValue(newValue)}</TableCell>
                </TableRow>
            )
        })
    };

    const renderAuditValue = (auditValue) => {
        return auditValue && auditValue.split(',').map((thisValue, index) => {
            return(
                <Text key={index}>{thisValue}</Text>
            )
        });
    };

    const renderAuditName = (auditName) => {
        return auditName && auditName.replace(/([A-Z])/g, ' $1').trim();
    };

    const getIdentifierName = (recordType) => {
        return recordType === 'Permission Policy' ? 'Directive' : 'Source';
    };

    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearchTerm(searchTerm);
        }, 500);
        return () => clearTimeout(timer);
    }, [searchTerm]);

    useEffect(() => {
        if (!mounted){
            getAuditUsers();
            setMonthStart();
            setMounted(true);
        }
        else{
            getAuditHistory();
        }
    }, [mounted, startDate, endDate, selectedUser, selectedOperationType, selectedRecordType, selectedPageSize, selectedFrom, debouncedSearchTerm])

    return (
        <>
            <Box className='security-page-container'>
                <Field label='Free Text Search' mb='16'>
                    <Input type='text' value={searchTerm} onChange={handleSearchTermChange} placeholder='Url fragment or value' />
                </Field>
                <Grid gridTemplateColumns={{ base: '1', md: '3' }} gap='16' mb='16'>
                    <Field label='User'>
                        <FormSelect value={selectedUser} onValueChange={handleSelectUser} options={userOptions} />
                    </Field>
                    <Field label='Operation'>
                        <FormSelect value={selectedOperationType} onValueChange={handleSelectOperationType} options={operationTypeOptions} />
                    </Field>
                    <Field label='Record'>
                        <FormSelect value={selectedRecordType} onValueChange={handleSelectRecordType} options={recordTypeOptions} />
                    </Field>
                </Grid>
                <Grid gridTemplateColumns={{ base: '1', md: '3' }} gap='16' mb='16'>
                    <Field label='Date Range'>
                        <DateRangePicker value={{ from: startDate, to: endDate }} onValueChange={handleSelectDateRange}>
                            <DateRangePickerTrigger w='full' />
                            <DateRangePickerContent />
                        </DateRangePicker>
                    </Field>
                    <Field label='Page Size'>
                        <FormSelect value={selectedPageSize} onValueChange={handleSelectPageSize} options={pageSizeOptions} />
                    </Field>
                </Grid>
                {renderAuditHistoryCards()}
                <Group justifyContent='center' my='16'>
                    <Button appearance='primary' size='lg' onClick={handleLoadMore}>Load More</Button>
                </Group>
            </Box>
        </>
    )
}

AuditHistory.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default AuditHistory
