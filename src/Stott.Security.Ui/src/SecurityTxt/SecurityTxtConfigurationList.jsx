import { useState, useEffect } from 'react';
import axios from 'axios';
import { Alert, Container, Row } from 'react-bootstrap';
import EditSiteSecurityTxt from './EditSiteSecurityTxt';
import DeleteSiteSecurityTxt from './DeleteSiteSecurityTxt';
import AddSiteSecurityTxt from './AddSiteSecurityTxt';

function SecurityTxtConfigurationList(props)
{

    const [siteCollection, setSiteCollection] = useState([])

    useEffect(() => {
        getSiteCollection()
    }, [])

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description)

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
            const { id, siteId, siteName, isForWholeSite, specificHost } = siteDetails
            const hostName = isForWholeSite === true ? 'Default' : specificHost;
            return (
                <tr key={index}>
                    <td>{siteName}</td>
                    <td>{hostName}</td>
                    <td>
                        <EditSiteSecurityTxt id={id} siteId={siteId} showToastNotificationEvent={props.showToastNotificationEvent} reloadEvent={getSiteCollection}></EditSiteSecurityTxt>
                        <DeleteSiteSecurityTxt id={id} siteName={siteName} showToastNotificationEvent={props.showToastNotificationEvent} reloadEvent={getSiteCollection}></DeleteSiteSecurityTxt>
                    </td>
                </tr>
            )
        })
    }

    return(
        <Container className='mt-3'>
            <Row className='mb-2'>
                <div className='col-xl-9 col-lg-9 col-sm-12 col-xs-12 p-0'>
                    <Alert variant='primary' className='p-3'>Read about security.txt files <a href='https://securitytxt.org/' target='_blank' rel='noopener noreferrer'>here</a>, and use their generator to create your own to be saved here.</Alert>
                </div>
                <div className='col-xl-3 col-lg-3 col-sm-12 col-xs-12 p-0 text-end'>
                    <AddSiteSecurityTxt showToastNotificationEvent={props.showToastNotificationEvent} reloadEvent={getSiteCollection}></AddSiteSecurityTxt>
                </div>
            </Row>
            <Row>
                <table className='table table-striped'>
                    <thead>
                        <tr>
                            <th className='table-header-fix'>Site Name</th>
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
    )
}

export default SecurityTxtConfigurationList