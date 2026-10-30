import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { httpGet } from '../Common/httpClient';
import EditPermission from './EditPermission'
import InheritedPermission from './InheritedPermission';
import AddPermission from './AddPermission';
import { Box, Flex, Table, TableBody, TableHeader, TableHeaderCell, TableRow } from '@optiaxiom/react';
import SourceFilter from './SourceFilter';

const PermissionList = (props) => {

    const [cspSources, setSources] = useState([])

    const getCspSources = async (sourceQuery, directiveQuery) => {
        await httpGet(import.meta.env.VITE_PERMISSION_LIST_URL, { source: sourceQuery, directive: directiveQuery, appId: props.appId, hostName: props.hostName })
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

    const renderPermissionList = () => {
        return cspSources && cspSources.map(cspSource => {
            const { id, isInherited, isDescendant } = cspSource
            if (isInherited || isDescendant) {
                return (<InheritedPermission key={id} sourceData={cspSource} />)
            }
            return (<EditPermission key={id} sourceData={cspSource} reloadSourceEvent={getCspSources} showToastNotificationEvent={props.showToastNotificationEvent} />)
        })
    };

    const handleSourceFilterChange = (source, directive) => getCspSources(source, directive);

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    useEffect(() => {
        getCspSources('', '');
    }, [props.appId, props.hostName]);

    return(
        <div>
            <Flex flexDirection={{ base: 'column', md: 'row' }} alignItems={{ base: 'stretch', md: 'end' }} gap='16' mb='16'>
                <AddPermission reloadSourceEvent={getCspSources} showToastNotificationEvent={props.showToastNotificationEvent} appId={props.appId} hostName={props.hostName}></AddPermission>
                <Box flex='1'>
                    <SourceFilter onSourceFilterUpdate={handleSourceFilterChange}></SourceFilter>
                </Box>
            </Flex>
            <Table>
                <TableHeader>
                    <TableRow>
                        <TableHeaderCell>Source</TableHeaderCell>
                        <TableHeaderCell>Directives</TableHeaderCell>
                        <TableHeaderCell>Actions</TableHeaderCell>
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {renderPermissionList()}
                </TableBody>
            </Table>
        </div>
    )
}

PermissionList.propTypes = {
    showToastNotificationEvent: PropTypes.func,
    appId: PropTypes.string,
    hostName: PropTypes.string
};

export default PermissionList;
