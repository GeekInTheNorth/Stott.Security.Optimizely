import { useCsp } from "./CspContext";
import CspPolicyHelper from "./helpers/CspPolicyHelper";
import { Button, Container } from "react-bootstrap";

function CspList() {
    const { allPolicies, selectPolicy } = useCsp();

    if (!allPolicies || allPolicies.length === 0) {
        return <div>No items to display.</div>;
    }

    return (
        <Container fluid>
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Policy Name</th>
                        <th>Scope</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {allPolicies.map((item, idx) => (
                        <tr key={idx}>
                            <td className='align-middle'>{item.name}</td>
                            <td className='align-middle'>{CspPolicyHelper.getScopeDescription(item)}</td>
                            <td className='align-middle'>{CspPolicyHelper.getStatusDescription(item)}</td>
                            <td>
                                <Button variant='primary' onClick={() => selectPolicy(item)}>Edit</Button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </Container>
    );
}

export default CspList;