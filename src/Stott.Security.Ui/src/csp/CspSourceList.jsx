import { useState, useEffect } from 'react';
import { Container } from "react-bootstrap";
import axios from 'axios';
import PropTypes from 'prop-types';
import CspSourceFilter from "./CspSourceFilter";
import CspSourceEdit from './CspSourceEdit';
import CspSourceCreate from './CspSourceCreate';

function CspSourceList(props) {
  const [cspSources, setSources] = useState([])
  const [queryFilter, setQueryFilter] = useState({ source: '', directive: '' });
  
  useEffect(() => {
    getCspSources('', '')
  }, []);
  
  const getCspSources = async (sourceQuery, directiveQuery) => {
    setQueryFilter({ source: sourceQuery, directive: directiveQuery });
    await axios.get(import.meta.env.VITE_APP_CSP_SOURCES_LIST, { params: { source: sourceQuery, directive: directiveQuery } })
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
    return cspSources && cspSources.map((cspSource) => {
      const { id, source, directives } = cspSource
      return (
        <CspSourceEdit id={id} source={source} directives={directives} key={id} reloadSourceEvent={() => getCspSources(queryFilter.source, queryFilter.directive)} showToastNotificationEvent={props.showToastNotificationEvent} />
      )
    })
  };
  
  const handleSourceFilterChange = (source, directive) => getCspSources(source, directive);
  
  const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);
  
  return(
    <div>
      <Container fluid>
        <div className='row'>
          <div className='col-md-2 col-xs-12 mb-3'>
            <CspSourceCreate reloadSourceEvent={getCspSources} showToastNotificationEvent={props.showToastNotificationEvent}></CspSourceCreate>
          </div>
          <div className='col-md-10 col-xs-12 mb-3'>
            <CspSourceFilter onSourceFilterUpdate={handleSourceFilterChange}></CspSourceFilter>
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

CspSourceList.propTypes = {
  showToastNotificationEvent: PropTypes.func.isRequired
};

export default CspSourceList;