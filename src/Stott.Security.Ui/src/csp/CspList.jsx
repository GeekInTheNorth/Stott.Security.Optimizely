import React from "react";
import { useCsp } from "./CspContext";
import CspPolicyHelper from "./helpers/CspPolicyHelper";

function CspList() {
    const { allPolicies } = useCsp();

    if (!allPolicies || allPolicies.length === 0) {
        return <div>No items to display.</div>;
    }

    return (
        <table className='sso-table'>
            <thead>
                <tr>
                    <th>Policy Name</th>
                    <th>Scope</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                {allPolicies.map((item, idx) => (
                    <tr key={idx}>
                        <td>{item.name}</td>
                        <td>{CspPolicyHelper.getScopeDescription(item)}</td>
                        <td>
                            <button className='btn-primary' onClick={() => console.log(`Edit ${item.name}`)}>Edit</button>
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
}

export default CspList;