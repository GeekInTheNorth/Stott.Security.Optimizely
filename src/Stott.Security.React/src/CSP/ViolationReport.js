import { useState, useEffect } from "react";
import axios from 'axios';
import ConvertCspViolation from "./ConvertCspViolation";
import { format } from "date-fns";
import { Container, Alert } from "react-bootstrap";
import SourceFilter from "./SourceFilter";

const ViolationReport = (props) => {

    const [cspViolations, setcspViolations] = useState([])
    const [isReportingEnabled, setIsReportingEnabled] = useState(false);

    useEffect(() => {
        getCspViolations('', '');
        getReportingState();
    },[])

    const getCspViolations = async (sourceQuery, directiveQuery) => {
        await axios.get(process.env.REACT_APP_VIOLATIONREPORT_LIST_URL, { params: { source: sourceQuery, directive: directiveQuery } })
            .then((response) => {
                setcspViolations(response.data);
            },
            () => {
                handleShowFailureToast('Error', 'Failed to load the Content Security Policy violation history.');
            });
    }

    const getReportingState = async () => {
        await axios.get(process.env.REACT_APP_SETTINGS_GET_URL)
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
                <tr key={key}>
                    <td>{source}</td>
                    <td>{directive}</td>
                    <td>{violations}</td>
                    <td>{format(new Date(lastViolated), "yyyy-MM-dd HH:mm:ss")}</td>
                    <td>
                        <ConvertCspViolation
                            cspViolationUrl={sanitizedSource}
                            cspViolationDirective={directive}
                            cspSourceSuggestions={sourceSuggestions}
                            cspDirectiveSuggestions={directiveSuggestions}
                            showToastNotificationEvent={props.showToastNotificationEvent}></ConvertCspViolation>
                    </td>
                </tr>
            )
        })
    }

    return(
        <div>
            { isReportingEnabled ? <Container className="mb-3"><Alert variant='primary'>Please note that new violations of the Content Security Policy (CSP) may take several minutes to appear depending on the browser.</Alert></Container> : '' }
            { isReportingEnabled ? '' : <Container className="mb-3"><Alert variant='warning'>Please note that internal reporting for this module is currently disabled and no further violations will be recorded.</Alert></Container> }
            <Container fluid className="mb-3">
                <SourceFilter onSourceFilterUpdate={handleSourceFilterChange}></SourceFilter>
            </Container>
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Source</th>
                        <th>Directive</th>
                        <th>Violations</th>
                        <th>Last Violated</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {renderViolationList()}
                </tbody>
            </table>
        </div>
    )

}

export default ViolationReport;