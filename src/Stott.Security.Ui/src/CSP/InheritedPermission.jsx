import PropTypes from 'prop-types';
import DirectivesTable from './DirectivesTable';

function InheritedPermission({ sourceData }) {
    return (
        <tr className="table-secondary">
            <td>{sourceData.source}</td>
            <td data-all-directives={sourceData.directives}>
                <DirectivesTable directives={sourceData.directives} />
            </td>
            <td><span className="badge bg-secondary rounded-pill lh-sm text-wrap">{sourceData.isInherited ? sourceData.inheritedLabel : sourceData.descendantLabel}</span></td>
        </tr>
    );
}

InheritedPermission.propTypes = {
    sourceData: PropTypes.shape({
        id: PropTypes.string.isRequired,
        source: PropTypes.string.isRequired,
        directives: PropTypes.string,
        appId: PropTypes.string,
        hostName: PropTypes.string,
        isInherited: PropTypes.bool,
        isDescendant: PropTypes.bool,
        inheritedLabel: PropTypes.string,
        descendantLabel: PropTypes.string
    }).isRequired,
};

export default InheritedPermission;
