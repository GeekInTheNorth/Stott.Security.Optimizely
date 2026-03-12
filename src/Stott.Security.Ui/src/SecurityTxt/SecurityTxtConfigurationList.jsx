import { useState, useEffect } from 'react';
import axios from 'axios';
import { Alert, Button, Container, Row } from 'react-bootstrap';
import EditSiteSecurityTxt from './EditSiteSecurityTxt';
import AddSiteSecurityTxt from './AddSiteSecurityTxt';
import ConfirmationModal from '../Common/ConfirmationModal';

function SecurityTxtConfigurationList(props) {

    const [siteCollection, setSiteCollection] = useState([]);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [itemToDelete, setItemToDelete] = useState(null);

    useEffect(() => {
        getSiteCollection();
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
        await axios.delete(url)
            .then(() => {
                handleShowSuccessToast('Success', `Your security.txt content for '${appName}' was successfully deleted.`);
                getSiteCollection();
            },
            () => {
                handleShowFailureToast('Failure', `An error was encountered when trying to delete your security.txt content for '${appName}'.`);
            });
    };

    const getSiteCollection = async () => {
        
        setSiteCollection([]);

        await axios.get(import.meta.env.VITE_APP_SECURITYTXT_LIST)
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
                <tr key={index}>
                    <td>{appName}</td>
                    <td>{hostName}</td>
                    <td>
                        <EditSiteSecurityTxt id={id} appId={appId} showToastNotificationEvent={props.showToastNotificationEvent} reloadEvent={getSiteCollection} />
                        <Button variant='danger' className='text-nowrap' onClick={() => handleShowDeleteModal(id, appName)}>Delete</Button>
                    </td>
                </tr>
            );
        });
    };

    return(
        <>
            <Container fluid='xl' className='mt-3'>
                <Row className='mb-2'>
                    <div className='col-xl-9 col-lg-9 col-sm-12 col-xs-12 p-0'>
                        <Alert variant='primary' className='p-3'>Security.txt files are served on a path of <strong>/.well-known/security.txt</strong> and exist to inform security researchers how to report issues. Read more about security.txt files <a href='https://securitytxt.org/' target='_blank' rel='noopener noreferrer'>here</a> and optionally use their generator.</Alert>
                    </div>
                    <div className='col-xl-3 col-lg-3 col-sm-12 col-xs-12 p-0 text-end'>
                        <AddSiteSecurityTxt showToastNotificationEvent={props.showToastNotificationEvent} reloadEvent={getSiteCollection} />
                    </div>
                </Row>
                <Row>
                    <table className='table table-striped'>
                        <thead>
                            <tr>
                                <th className='table-header-fix'>Application Name</th>
                                <th className='table-header-fix'>Host</th>
                                <th className='table-header-fix'>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {renderSiteList()}
                        </tbody>
                    </table>
                </Row>
            </Container>
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