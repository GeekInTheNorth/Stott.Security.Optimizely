import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';
import EditPermission from './EditPermission'
import InheritedPermission from './InheritedPermission';
import AddPermission from './AddPermission';
import { Container } from 'react-bootstrap';
import SourceFilter from './SourceFilter';

const PermissionList = (props) => {

    const [cspSources, setSources] = useState([])
    const [inheritedSources, setInheritedSources] = useState([])

    const isContextSpecific = !!(props.appId || props.hostName);

    const getCspSources = async (sourceQuery, directiveQuery) => {
        await axios.get(import.meta.env.VITE_PERMISSION_LIST_URL, { params: { source: sourceQuery, directive: directiveQuery, appId: props.appId, hostName: props.hostName } })
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
    };

    const getInheritedSources = async () => {
        if (!isContextSpecific) {
            setInheritedSources([]);
            return;
        }

        await axios.get(import.meta.env.VITE_PERMISSION_LIST_INHERITED_URL, { params: { appId: props.appId, hostName: props.hostName } })
            .then((response) => {
                if (response.data && Array.isArray(response.data)){
                    setInheritedSources(response.data);
                }
            },
            () => {
                // Silently fail for inherited sources
            });
    };

    const renderPermissionList = () => {
        return cspSources && cspSources.map(cspSource => {
            const { id, source, directives } = cspSource
            return (
                <EditPermission id={id} source={source} directives={directives} key={id} reloadSourceEvent={getCspSources} showToastNotificationEvent={props.showToastNotificationEvent} appId={props.appId} hostName={props.hostName} />
            )
        })
    };

    const renderInheritedList = () => {
        return inheritedSources && inheritedSources.map((cspSource, index) => (
            <InheritedPermission key={`inherited-${index}`} source={cspSource.source} directives={cspSource.directives} />
        ));
    };

    const handleSourceFilterChange = (source, directive) => getCspSources(source, directive);

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    useEffect(() => {
        getCspSources('', '');
        getInheritedSources();
    }, [props.appId, props.hostName]);

    return(
        <div>
            <Container fluid>
                <div className='row'>
                    <div className='col-md-2 col-xs-12 mb-3'>
                        <AddPermission reloadSourceEvent={getCspSources} showToastNotificationEvent={props.showToastNotificationEvent} appId={props.appId} hostName={props.hostName}></AddPermission>
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
                    {isContextSpecific && inheritedSources.length > 0 && renderInheritedList()}
                    {renderPermissionList()}
                </tbody>
            </table>
        </div>
    )
}

PermissionList.propTypes = {
    showToastNotificationEvent: PropTypes.func,
    appId: PropTypes.string,
    hostName: PropTypes.string
};

export default PermissionList;
