import React, { useState, useEffect } from 'react';
import axios from 'axios';
import EditPermission from './EditPermission'

const PermissionList = () => {

    const [cspSources, setSources] = useState([])

    useEffect(() => {
        getCspSources()
    }, [])

    const getCspSources = async () => {
        const response = await axios.get('https://localhost:44344/CspPermissions/list/')
        setSources(response.data)
    }

    const renderPermissionList = () => {
        return cspSources && cspSources.map((cspSource, index) => {
            const { id, source, directives } = cspSource
            return (
                <EditPermission id={id} source={source} directives={directives} key={id} />
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