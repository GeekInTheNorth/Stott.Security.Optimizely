import React, { useState, useEffect } from "react";
import axios from 'axios';
import ConvertCspViolation from "./ConvertCspViolation";
import Moment from "react-moment";

const ViolationReport = (props) => {

    const [cspViolations, setcspViolations] = useState([])

    useEffect(() => {
        getCspViolations()
    },[])

    const getCspViolations = async () => {
        await axios.get(process.env.REACT_APP_VIOLATIONREPORT_LIST_URL)
            .then((response) => {
                setcspViolations(response.data);
            },
            () => {
                handleShowFailureToast('Error', 'Failed to load the Content Security Policy violation history.');
            });
    }

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const renderViolationList = () => {
        return cspViolations && cspViolations.map((cspViolation, index) => {
            const { key, source, sanitizedSource, sourceSuggestions, directive, directiveSuggestions, violations, lastViolated } = cspViolation
            return (
                <tr key={key}>
                    <td>{source}</td>
                    <td>{directive}</td>
                    <td>{violations}</td>
                    <td><Moment format="YYYY-MM-DD HH:mm:ss">{lastViolated}</Moment></td>
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
            <label>Violations reported within the last 30 days.</label>
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