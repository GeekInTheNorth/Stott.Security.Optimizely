import { useState, useEffect } from 'react';
import { httpGet, httpDelete } from '../Common/httpClient';
import { Alert, Box, Button, Flex, Group, Table, TableBody, TableCell, TableHeader, TableHeaderCell, TableRow } from '@optiaxiom/react';
import EditSiteSecurityTxt from './EditSiteSecurityTxt';
import AddSiteSecurityTxt from './AddSiteSecurityTxt';
import ConfirmationModal from '../Common/ConfirmationModal';

function SecurityTxtConfigurationList(props) {

    const [siteCollection, setSiteCollection] = useState([]);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [itemToDelete, setItemToDelete] = useState(null);

    useEffect(() => {
        getSecurityTxtList();
    }, []);

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const handleShowDeleteModal = (id, appName) => {
        setItemToDelete({ id, appName });
        setShowDeleteModal(true);
    };

    const handleCloseDeleteModal = () => {
        setShowDeleteModal(false);
        setItemToDelete(null);
    };

    const handleConfirmDelete = async () => {
        if (!itemToDelete) return;

        const { id, appName } = itemToDelete;
        setShowDeleteModal(false);
        setItemToDelete(null);

        let url = ''.concat(import.meta.env.VITE_APP_SECURITYTXT_DELETE, id, '/');
        await httpDelete(url)
            .then(() => {
                handleShowSuccessToast('Success', `Your security.txt content for '${appName}' was successfully deleted.`);
                getSecurityTxtList();
            },
            () => {
                handleShowFailureToast('Failure', `An error was encountered when trying to delete your security.txt content for '${appName}'.`);
            });
    };

    const getSecurityTxtList = async () => {
        
        setSiteCollection([]);

        await httpGet(import.meta.env.VITE_APP_SECURITYTXT_LIST)
            .then((response) => {
                if (response.data && response.data.list && Array.isArray(response.data.list)){
                    setSiteCollection(response.data.list);
                }
                else{
                    handleShowFailureToast('Failure', 'Failed to retrieve security.txt configuration data.');
                }
            },
            () => {
                handleShowFailureToast('Failure', 'Failed to retrieve security.txt configuration data.');
            });
    }

    const renderSiteList = () => {
        return siteCollection && siteCollection.map((siteDetails, index) => {
            const { id, appId, appName, isForWholeSite, specificHost } = siteDetails;
            const hostName = isForWholeSite === true ? 'Default' : specificHost;
            return (
                <TableRow key={index}>
                    <TableCell>{appName}</TableCell>
                    <TableCell>{hostName}</TableCell>
                    <TableCell>
                        <Group gap='8'>
                            <EditSiteSecurityTxt id={id} appId={appId} showToastNotificationEvent={props.showToastNotificationEvent} reloadEvent={getSecurityTxtList} />
                            <Button appearance='danger' onClick={() => handleShowDeleteModal(id, appName)}>Delete</Button>
                        </Group>
                    </TableCell>
                </TableRow>
            );
        });
    };

    return(
        <>
            <Box className='security-page-container' mt='16'>
                <Flex flexDirection={{ base: 'column', md: 'row' }} alignItems={{ base: 'stretch', md: 'start' }} gap='16' mb='16'>
                    <Alert intent='information' flex='1'>Security.txt files are served on a path of <strong>/.well-known/security.txt</strong> and exist to inform security researchers how to report issues. Read more about security.txt files <a href='https://securitytxt.org/' target='_blank' rel='noopener noreferrer'>here</a> and optionally use their generator.</Alert>
                    <AddSiteSecurityTxt showToastNotificationEvent={props.showToastNotificationEvent} reloadEvent={getSecurityTxtList} />
                </Flex>
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHeaderCell>Application Name</TableHeaderCell>
                            <TableHeaderCell>Host</TableHeaderCell>
                            <TableHeaderCell>Actions</TableHeaderCell>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {renderSiteList()}
                    </TableBody>
                </Table>
            </Box>
            <ConfirmationModal
                show={showDeleteModal}
                title='Delete Security.txt Configuration'
                message={`Are you sure you want to delete this configuration for '${itemToDelete?.appName}'?`}
                confirmLabel='Delete'
                onConfirm={handleConfirmDelete}
                onCancel={handleCloseDeleteModal}
            />
        </>
    );
}

export default SecurityTxtConfigurationList