import PropTypes from 'prop-types';
import { Badge, TableCell, TableRow } from '@optiaxiom/react';
import DirectivesTable from './DirectivesTable';

function InheritedPermission({ sourceData }) {
    return (
        <TableRow bg="bg.secondary">
            <TableCell>{sourceData.source}</TableCell>
            <TableCell data-all-directives={sourceData.directives}>
                <DirectivesTable directives={sourceData.directives} />
            </TableCell>
            <TableCell><Badge intent="neutral">{sourceData.isInherited ? sourceData.inheritedLabel : sourceData.descendantLabel}</Badge></TableCell>
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
