import PropTypes from 'prop-types';
import DirectivesTable from './DirectivesTable';

function InheritedPermission({ source, directives }) {
    return (
        <tr className="table-secondary">
            <td>{source}</td>
            <td data-all-directives={directives}>
                <DirectivesTable directives={directives} />
            </td>
            <td><span className="badge bg-secondary">Inherited</span></td>
        </tr>
    );
}

InheritedPermission.propTypes = {
    source: PropTypes.string.isRequired,
    directives: PropTypes.string
};

export default InheritedPermission;
