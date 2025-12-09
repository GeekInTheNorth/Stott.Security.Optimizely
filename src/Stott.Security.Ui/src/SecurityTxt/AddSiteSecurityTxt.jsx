import { useState, useEffect } from 'react'
import axios from 'axios';
import { Alert, Button, Modal } from 'react-bootstrap'

function AddSiteSecurityTxt(props) {

    const [showModal, setShowModal] = useState(false);
    const [siteCollection, setSiteCollection] = useState([]);
    const [hostCollection, setHostCollection] = useState([]);
    const [siteId, setSiteId] = useState(null);
    const [siteName, setSiteName] = useState(null);
    const [siteContent, setSiteContent] = useState('');
    const [hostName, setHostName] = useState('');
    const [isDefault, setIsDefault] = useState(true)

    const handleCloseModal = () => {
        setShowModal(false);
    }

    const handleShowEditModal = async () => {
        await axios.get(import.meta.env.VITE_APP_SITES_LIST)
            .then((response) => {
                if (response.data && response.data && Array.isArray(response.data)){
                    setSiteCollection(response.data);
                    if(response.data.length > 0){
                        var firstSite = response.data[0];
                        var hosts = firstSite.availableHosts ?? [];
                        setSiteId(firstSite.siteId);
                        setSiteName(firstSite.siteName);
                        setHostCollection(hosts);
                        if (hosts.length > 0){
                            setHostName(hosts[0].value);
                        }
                    }

                    setSiteContent('');
                    setShowModal(true);
                }
                else{
                    handleShowFailureToast('Failure', 'Failed to retrieve site data.');
                }
            },
            () => {
                handleShowFailureToast('Failure', 'Failed to retrieve site data.');
            });
    }

    const handleSaveSecurityTxtContent = async () => {

        let selectedSite = getSelectedSite();
        let selectedHost = getSelectedHostName();
        let params = new URLSearchParams();
        params.append('siteId', selectedSite.siteId);
        params.append('siteName', selectedSite.siteName);
        params.append('specificHost', selectedHost);
        params.append('Content', siteContent);

        await axios.post(import.meta.env.VITE_APP_SECURITYTXT_SAVE, params)
            .then(() => {
                handleShowSuccessToast('Success', 'Your security.txt content changes for \'' + siteName + '\' were successfully applied.');
                setShowModal(false);
                handleReload();
            },
            (error) => {
                if (error.response && error.response.status === 409) {
                    handleShowFailureToast('Failure', error.response.data);
                    setShowModal(false);
                }
                else {
                    handleShowFailureToast('Failure', 'An error was encountered when trying to save your security.txt content.');
                    setShowModal(false);
                }
            });
    }

    const handleSiteSelection = (event) => {
        const selectedSiteid = event.target.value;
        const selectedSite = siteCollection.filter(x => x.siteId == selectedSiteid)[0];
        const availableHosts = selectedSite.availableHosts ?? [];
        const firstHost = availableHosts.length > 0 ? availableHosts[0].value : '';

        setSiteId(selectedSite.siteId);
        setSiteName(selectedSite.siteName);
        setHostName(firstHost);
        setHostCollection(availableHosts);
    }

    const handleHostSelection = (event) => {
        let selectedHost = event.target.value ?? '';
        setHostName(selectedHost);
        setIsDefault(selectedHost === '');
    }

    const handleSiteContentChange = (event) => {
        setSiteContent(event.target.value);
    }

    const renderAvailableSites = () => {
        return siteCollection && siteCollection.map((site, index) => {
            const { siteId, siteName } = site
            return (
                <option key={index} value={siteId}>{siteName}</option>
            )
        })
    }

    const renderAvailableHosts = () => {
        return hostCollection && hostCollection.map((host, index) => {
            const { hostName, displayName } = host
            return (
                <option key={index} value={hostName}>{displayName}</option>
            )
        })
    }

    const getSelectedSite = () => {
        if (siteId === undefined || siteId === null || siteId === '') {
            var firstSite = siteCollection[0];
            setSiteId(firstSite.siteId);
            setSiteName(firstSite.siteName);

            return firstSite;
        }

        var matches = siteCollection.filter(matchSite);

        return matches[0];
    }

    const matchSite = (thisSite) => {
        return thisSite && thisSite.siteId && thisSite.siteId === siteId;
    }

    const getSelectedHostName = () => {
        if (hostName === undefined || hostName === null || hostCollection.length === 0){
            return '';
        }

        return hostName;
    }

    useEffect(() => { renderAvailableHosts() }, [hostCollection]);

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);
    const handleReload = () => props.reloadEvent && props.reloadEvent();

    return(
        <>
            <Button variant='primary' onClick={handleShowEditModal} className='text-nowrap p-3'>Add Configuration</Button>
            <Modal show={showModal} size='xl'>
                <Modal.Header closeButton onClick={handleCloseModal}>
                    <Modal.Title>Create Security.txt Configuration</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                <div className='mb-3'>
                        <label>Site</label>
                        <select className='form-control form-select' name='SpecificHost' onChange={handleSiteSelection}>{renderAvailableSites()}</select>
                    </div>
                    <div className='mb-3'>
                        <label>Host</label>
                        <select className='form-control form-select' name='SpecificHost' value={hostName} onChange={handleHostSelection}>{renderAvailableHosts()}</select>
                    </div>
                    <Alert variant='primary' show={isDefault} className='my-2 p-2'>
                        Please note that security.txt content for a host of 'Default' will be used where security.txt content has not been set for a specific host.
                    </Alert>
                    <div className='mb-3'>
                        <label>Security.txt Content</label>
                        <textarea className='form-control large-text-area' name='securityTxtContent' cols='60' rows='10' onChange={handleSiteContentChange} value={siteContent}></textarea>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant='primary' type='button' onClick={handleSaveSecurityTxtContent}>Save Changes</Button>
                    <Button variant='secondary' type='button' onClick={handleCloseModal}>Cancel</Button>
                </Modal.Footer>
            </Modal>
        </>
    )
}

export default AddSiteSecurityTxt;