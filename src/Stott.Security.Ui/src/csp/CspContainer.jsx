import CspList from "./CspList";
import { useCsp } from "./CspContext";
import CspSettings from "./CspSettings";
import { Container } from "react-bootstrap";

function CspContainer()
{
    const { viewMode, currentPolicy } = useCsp();

    return (
        <Container fluid>
            {viewMode === 'list' ? <CspList /> : null}
            {viewMode === 'edit' ? <CspSettings cspPolicy={currentPolicy} /> : null}
        </Container>
    );
}

export default CspContainer;