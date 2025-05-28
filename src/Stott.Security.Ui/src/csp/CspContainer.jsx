import CspList from "./CspList";
import { useCsp } from "./CspContext";
import CspSettings from "./CspSettings";
import CspBreadcrumb from "./CspBreadcrumb";
import CspSandbox from "./CspSandbox";
import CspSourceList from "./CspSourceList";
import CspViolationList from "./CspViolationList";

function CspContainer()
{
    const { viewMode, currentPolicy } = useCsp();

    return (
        <>
            <CspBreadcrumb />
            {viewMode === 'list' ? <CspList /> : null}
            {viewMode === 'settings' ? <CspSettings cspPolicy={currentPolicy} /> : null}
            {viewMode === 'sandbox' ? <CspSandbox /> : null}
            {viewMode === 'sources' ? <CspSourceList /> : null}
            {viewMode === 'violations' ? <CspViolationList /> : null}
        </>
    );
}

export default CspContainer;