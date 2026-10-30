import PropTypes from 'prop-types';
import { TableCell, TableRow } from '@optiaxiom/react';
import DirectivesTable from './DirectivesTable';

function InheritedPermission({ sourceData }) {
    return (
        <TableRow bg="bg.information.subtle">
            <TableCell>{sourceData.source}</TableCell>
            <TableCell data-all-directives={sourceData.directives}>
                <DirectivesTable directives={sourceData.directives} />
            </TableCell>
            <TableCell>{sourceData.isInherited ? sourceData.inheritedLabel : sourceData.descendantLabel}</TableCell>
        </TableRow>
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
