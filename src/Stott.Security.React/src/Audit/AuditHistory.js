import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Container, Form, Button, Card } from 'react-bootstrap';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import Moment from "react-moment";

function AuditHistory() {

    const [mounted, setMounted] = useState(false);
    const [auditUsers, setAuditUsers] = useState([])
    const [auditHistory, setAuditHistory] = useState([])

    const [startDate, setStartDate] = useState(new Date());
    const [endDate, setEndDate] = useState(new Date());

    const [selectedUser, setSelectedUser] = useState('')
    const [selectedOperationType, setSelectedOperationType] = useState('')
    const [selectedRecordType, setSelectedRecordType] = useState('')
    const [selectedFrom, setSelectedFrom] = useState(0)
    const [selectedPageSize, setSelectedPageSize] = useState(10)

    const setMonthStart = () => {
        var today = new Date();
        setStartDate(new Date(today.getFullYear(), today.getMonth(), today.getDate() - 7));
    }

    const handleSelectUser = (event) => {
        setSelectedFrom(0);
        setSelectedUser(event.target.value);
    }

    const handleSelectOperationType = (event) => {
        setSelectedFrom(0);
        setSelectedOperationType(event.target.value);
    }

    const handleSelectRecordType = (event) => {
        setSelectedFrom(0);
        setSelectedRecordType(event.target.value);
    }

    const handleSelectPageSize = (event) => {
        setSelectedFrom(0);
        setSelectedPageSize(event.target.value);
    }

    const handleSelectStartDate = (selectedDate) => {
        setSelectedFrom(0);
        setStartDate(selectedDate);
    }

    const handleSelectEndDate = (selectedDate) => {
        setSelectedFrom(0);
        setEndDate(selectedDate);
    }

    const handleLoadMore = () => {
        let newSelectedFrom = selectedFrom + selectedPageSize;
        setSelectedFrom(newSelectedFrom);
    }

    const getAuditHistory = async () => {
        const response = await axios.get(process.env.REACT_APP_AUDIT_LIST_URL, {params: {
            dateFrom: startDate,
            dateTo: endDate,
            actionedBy: selectedUser,
            recordType: selectedRecordType,
            operationType: selectedOperationType,
            from: selectedFrom,
            take: selectedPageSize
        }});
        if (selectedFrom === 0){
            setAuditHistory(response.data);
        }
        else {
            let additional = response.data;
            setAuditHistory(current => [...current, ...additional]);
        }
    }

    const getAuditUsers = async () => {
        const response = await axios.get(process.env.REACT_APP_AUDIT_USER_URL);
        setAuditUsers(response.data);
    }

    const renderAuditHistoryCards = () => {
        return auditHistory && auditHistory.map((auditEntry, index) => {
            const { id, actioned, actionedBy, operationType, recordType, identifier, changes } = auditEntry
            return(
                <Card id={id} className='my-3'>
                    <Card.Header><strong>{recordType}</strong> were <strong>{operationType}</strong> by <strong>{actionedBy}</strong> at <strong><Moment format="YYYY-MM-DD HH:mm:ss">{actioned}</Moment></strong></Card.Header>
                    <Card.Body>
                        {identifier === '' ? '' : <Card.Subtitle className='mb-3'>{operationType} Source: {identifier}</Card.Subtitle>}
                        <table className='table table-striped'>
                            <thead>
                                <th>Field</th>
                                <th>Old Value</th>
                                <th>New Value</th>
                            </thead>
                            <tbody>
                                {renderAuditHistoryCardDetails(changes)}
                            </tbody>
                        </table>
                    </Card.Body>
                </Card>
            )
        })
    }

    const renderAuditHistoryCardDetails = (auditChanges) => {
        return auditChanges && auditChanges.map((auditChange, index) => {
            const { id, field, oldValue, newValue } = auditChange
            return(
                <tr key={id}>
                    <td>{field}</td>
                    <td class='allow-word-break'>{oldValue}</td>
                    <td class='allow-word-break'>{newValue}</td>
                </tr>
            )
        })
    }

    useEffect(() => {
        if (!mounted){
            getAuditUsers();
            setMonthStart();
            setMounted(true);
        }
        else{
            getAuditHistory();
        }
    }, [mounted, startDate, endDate, selectedUser, selectedOperationType, selectedRecordType, selectedPageSize, selectedFrom])

    return (
        <>
            <Container>
                <div className='row mb-3'>
                    <div className='col-md-4 col-xs-12'>
                        <Form.Group>
                            <Form.Label id='lblSelectUser'>User</Form.Label>
                            <Form.Select value={selectedUser} onChange={handleSelectUser} aria-describedby='lblSelectUser' className='form-control'>
                                <option value=''>All</option>
                                {auditUsers.map(user => (
                                    <option key={user} value={user}>{user}</option>
                                ))}
                            </Form.Select>
                        </Form.Group>
                    </div>
                    <div className='col-md-4 col-xs-12'>
                        <Form.Group>
                            <Form.Label id='lblSelectOperationType'>Operation</Form.Label>
                            <Form.Select value={selectedOperationType} onChange={handleSelectOperationType} aria-describedby='lblSelectOperationType' className='form-control'>
                                <option value=''>All</option>
                                <option value='Added'>Added</option>
                                <option value='Deleted'>Deleted</option>
                                <option value='Modified'>Modified</option>
                            </Form.Select>
                        </Form.Group>
                    </div>
                    <div className='col-md-4 col-xs-12'>
                        <Form.Group>
                            <Form.Label id='lblSelectRecordType'>Record</Form.Label>
                            <Form.Select value={selectedRecordType} onChange={handleSelectRecordType} aria-describedby='lblSelectRecordType' className='form-control'>
                                <option value=''>All</option>
                                <option value='CORS Settings'>CORS Settings</option>
                                <option value='CSP Settings'>CSP Settings</option>
                                <option value='CSP Source'>CSP Source</option>
                                <option value='CSP Sandbox'>CSP Sandbox</option>
                                <option value='Security Header Settings'>Security Header Settings</option>
                            </Form.Select>
                        </Form.Group>
                    </div>
                </div>
                <div className='row mb-3'>
                    <div className='col-md-4 col-xs-12'>
                        <Form.Group>
                            <Form.Label id='lblSelectDateFrom'>From Date</Form.Label>
                            <DatePicker selected={startDate} onChange={(date) => handleSelectStartDate(date)} className='form-control' ariaDescribedBy='lblSelectDateFrom' />
                        </Form.Group>
                    </div>
                    <div className='col-md-4 col-xs-12'>
                        <Form.Group>
                            <Form.Label id='lblSelectDateTo'>To Date</Form.Label>
                            <DatePicker selected={endDate} onChange={(date) => handleSelectEndDate(date)} className='form-control' ariaDescribedBy='lblSelectDateTo' />
                        </Form.Group>
                    </div>
                    <div className='col-md-4 col-xs-12'>
                        <Form.Group>
                            <Form.Label id='lblNumberOfRecords'>Page Size</Form.Label>
                            <Form.Select value={selectedPageSize} onChange={handleSelectPageSize} aria-describedby='lblNumberOfRecords' className='form-control'>
                                <option value={10}>10</option>
                                <option value={25}>25</option>
                                <option value={100}>100</option>
                            </Form.Select>
                        </Form.Group>
                    </div>
                </div>
            </Container>
            <Container>
                {renderAuditHistoryCards()}
            </Container>
            <Container>
                <div className='row'>
                    <div className='col-md-4 col-xs-12 offset-md-4'>
                        <Form.Group>
                            <Button variant='primary' onClick={handleLoadMore} className='form-control'>Load More</Button>
                        </Form.Group>
                    </div>
                </div>
            </Container>
        </>
    )
}

export default AuditHistory