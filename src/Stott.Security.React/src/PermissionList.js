import React, { useState, useEffect } from 'react';
import axios from 'axios';
import EditPermission from './EditPermission'
import AddPermission from './AddPermission';

const PermissionList = (props) => {

    const [cspSources, setSources] = useState([])

    useEffect(() => {
        getCspSources()
    }, [])

    const getCspSources = async () => {
        const response = await axios.get(process.env.REACT_APP_PERMISSION_LIST_URL)
        setSources(response.data)
    }

    const renderPermissionList = () => {
        return cspSources && cspSources.map((cspSource, index) => {
            const { id, source, directives } = cspSource
            return (
                <EditPermission id={id} source={source} directives={directives} key={id} reloadSourceEvent={getCspSources} showToastNotificationEvent={props.showToastNotificationEvent} />
            )
        })
    }

    return(
        <div>
            <AddPermission reloadSourceEvent={getCspSources} showToastNotificationEvent={props.showToastNotificationEvent}></AddPermission>
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