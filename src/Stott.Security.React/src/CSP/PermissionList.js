import React, { useState, useEffect } from 'react';
import axios from 'axios';
import EditPermission from './EditPermission'
import AddPermission from './AddPermission';
import { Container, Form, InputGroup } from 'react-bootstrap';

const PermissionList = (props) => {

    const [cspSources, setSources] = useState([])
    const [sourceFilter, setSourceFilter] = useState("")
    const [directiveFilter, setDirectiveFilter] = useState("")

    useEffect(() => {
        getCspSources('', '')
    }, [])

    const getCspSources = async (sourceQuery, directiveQuery) => {
        await axios.get(process.env.REACT_APP_PERMISSION_LIST_URL, { params: { source: sourceQuery, directive: directiveQuery } })
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

    const handleSourceFilter = (event) => {
        setSourceFilter(event.target.value);
        getCspSources(event.target.value, directiveFilter);
    }

    const handleDirectiveFilter = (event) => {
        setDirectiveFilter(event.target.value);
        getCspSources(sourceFilter, event.target.value);
    }

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    return(
        <div>
            <Container fluid>
                <div className='row'>
                    <div className='col-md-2 col-xs-12 mb-3'>
                        <AddPermission reloadSourceEvent={getCspSources} showToastNotificationEvent={props.showToastNotificationEvent}></AddPermission>
                    </div>
                    <div className='col-md-10 col-xs-12 mb-3'>
                        <InputGroup>
                            <InputGroup.Text id='lblSourceFilters'>Filter</InputGroup.Text>
                            <Form.Control id='txtSourceFilter' type='text' value={sourceFilter} onChange={handleSourceFilter} aria-describedby='lblSourceFilters' placeholder='Type a partial url'></Form.Control>
                            <Form.Select value={directiveFilter} onChange={handleDirectiveFilter} aria-describedby='lblSourceFilters' className='form-control'>
                                <option value=''>Any Directive</option>
                                <option value='base-uri'>base-uri</option>
                                <option value='default-src'>default-src</option>
                                <option value='child-src'>child-src</option>
                                <option value='frame-src'>frame-src</option>
                                <option value='frame-ancestors'>frame-ancestors</option>
                                <option value='connect-src'>connect-src</option>
                                <option value='navigate-to'>navigate-to</option>
                                <option value='form-action'>form-action</option>
                                <option value='font-src'>font-src</option>
                                <option value='img-src'>img-src</option>
                                <option value='media-src'>media-src</option>
                                <option value='object-src'>object-src</option>
                                <option value='manifest-src'>manifest-src</option>
                                <option value='prefetch-src'>prefetch-src</option>
                                <option value='script-src'>script-src</option>
                                <option value='script-src-elem'>script-src-elem</option>
                                <option value='script-src-attr'>script-src-attr</option>
                                <option value='worker-src'>worker-src</option>
                                <option value='style-src'>style-src</option>
                                <option value='style-src-elem'>style-src-elem</option>
                                <option value='style-src-attr'>style-src-attr</option>
                            </Form.Select>
                        </InputGroup>
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