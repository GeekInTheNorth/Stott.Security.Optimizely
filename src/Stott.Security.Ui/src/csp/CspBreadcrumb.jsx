import { useCsp } from "./CspContext";
import Breadcrumb from 'react-bootstrap/Breadcrumb';

function CspBreadcrumb() {

    const { viewMode, selectListMode, currentPolicy } = useCsp();

    const handleViewList = (e) => {
        e.preventDefault();
        selectListMode();
    }

    if (viewMode === 'list') {
        return (
            <Breadcrumb>
                <Breadcrumb.Item active className="csp-breadcrumb-item">
                    All Policies
                </Breadcrumb.Item>
            </Breadcrumb>
        );
    } else if (viewMode === 'edit' && currentPolicy) {
        return (
            <Breadcrumb>
                <Breadcrumb.Item href="#" onClick={handleViewList}>
                    All Policies
                </Breadcrumb.Item>
                <Breadcrumb.Item active className="csp-breadcrumb-item">
                    {currentPolicy.name}
                </Breadcrumb.Item>
            </Breadcrumb>
        );
    } else {
        return null;
    }
}

export default CspBreadcrumb;