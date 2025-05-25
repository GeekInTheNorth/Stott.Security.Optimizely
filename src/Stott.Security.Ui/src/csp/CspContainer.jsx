import CspList from "./CspList";
import { useCsp } from "./CspContext";
import CspSettings from "./CspSettings";

function CspContainer()
{
    const { viewMode, currentPolicy } = useCsp();

    return (
        <div className='sso-settings'>
            {viewMode === 'list' ? <CspList /> : null}
            {viewMode === 'edit' ? <CspSettings cspPolicy={currentPolicy} /> : null}
        </div>
    );
}

export default CspContainer;