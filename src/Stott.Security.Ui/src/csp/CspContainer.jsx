import CspList from "./CspList";
import { useCsp } from "./CspContext";
import CspSettings from "./CspSettings";
import CspBreadcrumb from "./CspBreadcrumb";
import CspSandbox from "./CspSandbox";
import CspSourceList from "./CspSourceList";
import CspViolationList from "./CspViolationList";
import PropTypes from 'prop-types';

function CspContainer({ showToastNotificationEvent }) {
    const { viewMode, currentPolicy } = useCsp();

    return (
        <>
            <CspBreadcrumb />
            {viewMode === 'list' ? <CspList /> : null}
            {viewMode === 'settings' ? <CspSettings cspPolicy={currentPolicy} showToastNotificationEvent={showToastNotificationEvent} /> : null}
            {viewMode === 'sandbox' ? <CspSandbox showToastNotificationEvent={showToastNotificationEvent} /> : null}
            {viewMode === 'sources' ? <CspSourceList /> : null}
            {viewMode === 'violations' ? <CspViolationList showToastNotificationEvent={showToastNotificationEvent} /> : null}
        </>
    );
}

CspContainer.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default CspContainer;