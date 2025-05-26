import CspList from "./CspList";
import { useCsp } from "./CspContext";
import CspSettings from "./CspSettings";
import { Container } from "react-bootstrap";
import CspBreadcrumb from "./CspBreadcrumb";

function CspContainer()
{
    const { viewMode, currentPolicy } = useCsp();

    return (
        <Container fluid>
            <CspBreadcrumb />
            {viewMode === 'list' ? <CspList /> : null}
            {viewMode === 'edit' ? <CspSettings cspPolicy={currentPolicy} /> : null}
        </Container>
    );
}

export default CspContainer;