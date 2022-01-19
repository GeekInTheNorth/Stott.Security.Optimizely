import React, { useState, useEffect } from 'react';
import axios from 'axios';
import EditPermission from './EditPermission'

const PermissionList = () => {

    const [cspSources, setSources] = useState([])

    useEffect(() => {
        getCspSources()
    }, [])

    const getCspSources = async () => {
        const response = await axios.get('https://localhost:44344/CspPermissions/list/json')
        setSources(response.data)
    }

    const renderPermissionList = () => {
        return cspSources && cspSources.map((cspSource, index) => {
            const { id, source, directives } = cspSource
            return (
                <tr key={id}>
                    <td>{source}</td>
                    <td>{directives}</td>
                    <td>
                        <EditPermission id={id} source={source} directives={directives} />
                    </td>
                </tr>
            )
        })
    }

    return(
        <div>
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