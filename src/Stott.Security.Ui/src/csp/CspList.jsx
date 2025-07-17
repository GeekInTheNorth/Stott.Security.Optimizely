import { useCsp } from "./CspContext";
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
                    {allPolicies.map((item) => (
                        <tr key={item.id}>
                            <td className='align-middle'>{item.name}</td>
                            <td className='align-middle'>{item.scope}</td>
                            <td className='align-middle'>{item.status}</td>
                            <td>
                                <Button variant='primary' className='me-2' onClick={() => selectPolicy(item, 'settings')}>Settings</Button>
                                <Button variant='outline-primary' className='me-2' onClick={() => selectPolicy(item, 'sources')}>Sources</Button>
                                <Button variant='outline-primary' className='me-2' onClick={() => selectPolicy(item, 'sandbox')}>Sandbox</Button>
                                <Button variant='outline-secondary' className='me-2' onClick={() => selectPolicy(item, 'violations')}>Violations</Button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </Container>
    );
}

export default CspList;