import { useState, useEffect } from "react";
import PropTypes from 'prop-types';
import { httpGet } from '../Common/httpClient';
import ConvertCspViolation from "./ConvertCspViolation";
import { format } from "date-fns";
import { Alert, Box, Table, TableBody, TableCell, TableHeader, TableHeaderCell, TableRow } from "@optiaxiom/react";
import SourceFilter from "./SourceFilter";

const ViolationReport = (props) => {

    const [cspViolations, setcspViolations] = useState([])
    const [isReportingEnabled, setIsReportingEnabled] = useState(false);

    useEffect(() => {
        getCspViolations('', '');
        getReportingState();
    },[props.appId, props.hostName])

    const getCspViolations = async (sourceQuery, directiveQuery) => {
        await httpGet(import.meta.env.VITE_VIOLATIONREPORT_LIST_URL, { source: sourceQuery, directive: directiveQuery, appId: props.appId, hostName: props.hostName })
            .then((response) => {
                setcspViolations(response.data);
            },
            () => {
                handleShowFailureToast('Error', 'Failed to load the Content Security Policy violation history.');
            });
    }

    const getReportingState = async () => {
        await httpGet(import.meta.env.VITE_SETTINGS_GET_URL, { appId: props.appId, hostName: props.hostName })
            .then((response) => {
                var isEnabled = response.data.isEnabled && response.data.useInternalReporting;
                setIsReportingEnabled(isEnabled);
            },
            () => {
                handleShowFailureToast('Error', 'Failed to load the Content Security Policy violation history.');
            });
    }

    const handleSourceFilterChange = (source, directive) => getCspViolations(source, directive);

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const renderViolationList = () => {
        return cspViolations && cspViolations.map(cspViolation => {
            const { key, source, sanitizedSource, sourceSuggestions, directive, directiveSuggestions, violations, lastViolated } = cspViolation
            return (
                <TableRow key={key}>
                    <TableCell>{source}</TableCell>
                    <TableCell>{directive}</TableCell>
                    <TableCell>{violations}</TableCell>
                    <TableCell>{format(new Date(lastViolated), "yyyy-MM-dd HH:mm:ss")}</TableCell>
                    <TableCell>
                        <ConvertCspViolation
                            cspViolationUrl={sanitizedSource}
                            cspViolationDirective={directive}
                            cspSourceSuggestions={sourceSuggestions}
                            cspDirectiveSuggestions={directiveSuggestions}
                            showToastNotificationEvent={props.showToastNotificationEvent}
                            appId={props.appId}
                            hostName={props.hostName}></ConvertCspViolation>
                    </TableCell>
                </TableRow>
            )
        })
    }

    return(
        <div>
            { isReportingEnabled ? <Alert intent='information' mb='16'>Please note that new violations of the Content Security Policy (CSP) may take several minutes to appear depending on the browser.</Alert> : '' }
            { isReportingEnabled ? '' : <Alert intent='warning' mb='16'>Please note that internal reporting for this module is currently disabled and no further violations will be recorded.</Alert> }
            <Box mb='16'>
                <SourceFilter onSourceFilterUpdate={handleSourceFilterChange}></SourceFilter>
            </Box>
            <Table>
                <TableHeader>
                    <TableRow>
                        <TableHeaderCell>Source</TableHeaderCell>
                        <TableHeaderCell>Directive</TableHeaderCell>
                        <TableHeaderCell>Violations</TableHeaderCell>
                        <TableHeaderCell>Last Violated</TableHeaderCell>
                        <TableHeaderCell>Actions</TableHeaderCell>
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {renderViolationList()}
                </TableBody>
            </Table>
        </div>
    )

}

ViolationReport.propTypes = {
    showToastNotificationEvent: PropTypes.func,
    appId: PropTypes.string,
    hostName: PropTypes.string
};

export default ViolationReport;
