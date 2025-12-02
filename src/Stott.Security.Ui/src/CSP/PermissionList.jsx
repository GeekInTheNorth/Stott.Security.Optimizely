import { useState, useEffect } from 'react';
import axios from 'axios';
import EditPermission from './EditPermission'
import AddPermission from './AddPermission';
import { Container } from 'react-bootstrap';
import SourceFilter from './SourceFilter';

const PermissionList = (props) => {

    const [cspSources, setSources] = useState([])

    useEffect(() => {
        getCspSources('', '')
    }, [])

    const getCspSources = async (sourceQuery, directiveQuery) => {
        await axios.get(import.meta.env.VITE_PERMISSION_LIST_URL, { params: { source: sourceQuery, directive: directiveQuery } })
            .then((response) => {
                if (response.data && Array.isArray(response.data)){
                    setSources(response.data);
                }
                else{
                    handleShowFailureToast("Get CSP Sources", "Failed to retrieve Content Security Policy Sources.");
                }
            },
            () => {
                handleShowFailureToast("Error", "Failed to retrieve the Content Security Policy Sources.");
            });
    }

    const renderPermissionList = () => {
        return cspSources && cspSources.map((cspSource, index) => {
            const { id, source, directives } = cspSource
            return (
                <EditPermission id={id} source={source} directives={directives} key={id} reloadSourceEvent={getCspSources} showToastNotificationEvent={props.showToastNotificationEvent} />
            )
        })
    }

    const handleSourceFilterChange = (source, directive) => getCspSources(source, directive);

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    return(
        <div>
            <Container fluid>
                <div className='row'>
                    <div className='col-md-2 col-xs-12 mb-3'>
                        <AddPermission reloadSourceEvent={getCspSources} showToastNotificationEvent={props.showToastNotificationEvent}></AddPermission>
                    </div>
                    <div className='col-md-10 col-xs-12 mb-3'>
                        <SourceFilter onSourceFilterUpdate={handleSourceFilterChange}></SourceFilter>
                    </div>
                </div>
            </Container>
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Source</th>
                        <th>Directives</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {renderPermissionList()}
                </tbody>
            </table>
        </div>
    )
}

export default PermissionList;
