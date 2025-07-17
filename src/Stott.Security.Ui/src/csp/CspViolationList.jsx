import { useState, useEffect } from "react";
import { useCsp } from "./CspContext";
import CspSourceFilter from "./CspSourceFilter";
import CspConvertViolation from "./CspConvertViolation";
import { Alert, Container } from "react-bootstrap";
import axios from "axios";
import PropTypes from 'prop-types';

function CspViolationList(props) {

  const { currentPolicy } = useCsp();

  const [cspViolations, setcspViolations] = useState([])

  useEffect(() => {
      getCspViolations('', '');
  },[])

  const getCspViolations = async (sourceQuery, directiveQuery) => {
    await axios.get(import.meta.env.VITE_APP_CSP_VIOLATIONS_LIST, { params: { policyId: currentPolicy.Id, source: sourceQuery, directive: directiveQuery } })
      .then((response) => {
          setcspViolations(response.data);
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
              <td>
                {lastViolated && (() => {
                  const date = new Date(lastViolated);
                  const pad = n => n.toString().padStart(2, '0');
                  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())} ${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`;
                })()}
              </td>
              <td>
                  <CspConvertViolation cspViolationUrl={sanitizedSource} cspViolationDirective={directive} cspSourceSuggestions={sourceSuggestions} cspDirectiveSuggestions={directiveSuggestions} showToastNotificationEvent={props.showToastNotificationEvent}></CspConvertViolation>
              </td>
          </tr>
        )
    })
  }

  return(
    <div>
      <Container className="mb-3">
        { currentPolicy.useInternalReporting ? 
          <Alert variant='primary'>Please note that new violations of the Content Security Policy (CSP) may take several minutes to appear depending on the browser.</Alert> :
          <Alert variant='warning'>Please note that internal reporting for this module is currently disabled and no further violations will be recorded.</Alert> }
      </Container>
      <Container fluid className="mb-3">
        <CspSourceFilter onSourceFilterUpdate={handleSourceFilterChange}></CspSourceFilter>
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
  );
}

CspViolationList.propTypes = {
  showToastNotificationEvent: PropTypes.func.isRequired
};

export default CspViolationList;