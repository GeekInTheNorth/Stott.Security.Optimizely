import { Container } from "react-bootstrap";
import { useCsp } from "./CspContext";
import Breadcrumb from 'react-bootstrap/Breadcrumb';

function CspBreadcrumb() {

    const { viewMode, selectListMode, selectPolicy, selectPolicyView, currentPolicy } = useCsp();

    const handleViewList = (e) => {
        e.preventDefault();
        selectListMode();
    }

    const renderAllPolicies = () => {
        if (viewMode === 'list') {
            return (
                <Breadcrumb.Item active className="csp-breadcrumb-item">All Policies</Breadcrumb.Item>
            );
        } else {
            return (
                <Breadcrumb.Item href="#" onClick={handleViewList}>All Policies</Breadcrumb.Item>
            );
        }
    }

    const renderSelectedPolicy = () => {
        if (viewMode === 'settings' && currentPolicy) {
            return (
                <Breadcrumb.Item active className="csp-breadcrumb-item">{currentPolicy.name}</Breadcrumb.Item>
            );
        } else if (viewMode !== 'list' && currentPolicy) {
            return (
                <Breadcrumb.Item href="#" onClick={() => selectPolicy(currentPolicy)}>{currentPolicy.name}</Breadcrumb.Item>
            );
        }

        return null;
    }

    const renderSandbox = () => {
        if (viewMode === 'sandbox' && currentPolicy) {
            return (
                <Breadcrumb.Item active className="csp-breadcrumb-item">Sandbox Settings</Breadcrumb.Item>
            );
        }
        return null;
    }

    const renderSources = () => {
        if (viewMode === 'sources' && currentPolicy) {
            return (
                <Breadcrumb.Item active className="csp-breadcrumb-item">Source List</Breadcrumb.Item>
            );
        }
        return null;
    }

    const renderViolations = () => {
        if (viewMode === 'violations' && currentPolicy) {
            return (
                <Breadcrumb.Item active className="csp-breadcrumb-item">Violation List</Breadcrumb.Item>
            );
        }
        return null;
    };

    const renderQuickNavigation = () => {
        if (viewMode === 'settings' && currentPolicy) {
            return (
                <>
                    <Breadcrumb.Item href="#" onClick={() => selectPolicyView('sources')}>Edit Sources</Breadcrumb.Item>
                    <Breadcrumb.Item href="#" onClick={() => selectPolicyView('sandbox')}>Edit Sandbox</Breadcrumb.Item>
                    <Breadcrumb.Item href="#" onClick={() => selectPolicyView('violations')}>View Violations</Breadcrumb.Item>
                </>
            );
        }

        return null;
    }

    return (
        <Container fluid>
            <Breadcrumb>
                {renderAllPolicies()}
                {renderSelectedPolicy()}
                {renderSandbox()}
                {renderSources()}
                {renderViolations()}
                {renderQuickNavigation()}
            </Breadcrumb>
        </Container>
    );
}

export default CspBreadcrumb;