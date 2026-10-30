import PropTypes from 'prop-types';
import { Button, Card, CardFooter, CardHeader, Text } from '@optiaxiom/react';

function CustomHeaderCard(props) {
    const headerName = props.header.headerName ?? '';
    const description = props.header.description ?? '';
    const behavior = props.header.behavior ?? 0;
    const headerValue = props.header.headerValue ?? '';
    const canDelete = props.header.canDelete ?? true;
    const isInherited = props.isInherited ?? false;

    const getBehaviorLabel = (behavior) => {
        switch (behavior) {
            case 0: return 'Disabled';
            case 1: return 'Add';
            case 2: return 'Remove';
            default: return 'Unknown';
        }
    };

    return (
        <Card my='16'>
            <CardHeader>{headerName}</CardHeader>
            {description && <Text><em>{description}</em></Text>}
            <Text><strong>Behavior:</strong> {getBehaviorLabel(behavior)}</Text>
            {behavior === 1 && (<Text><strong>Value:</strong> {headerValue}</Text>)}
            {!isInherited && (
                <CardFooter>
                    <Button appearance='primary' onClick={() => props.onEdit(props.header)}>Edit</Button>
                    {canDelete && (<Button appearance='danger' onClick={() => props.onDelete(props.header.id, props.header.headerName)}>Delete</Button>)}
                </CardFooter>
            )}
        </Card>
    );
}

CustomHeaderCard.propTypes = {
    header: PropTypes.shape({
        id: PropTypes.string,
        headerName: PropTypes.string,
        description: PropTypes.string,
        behavior: PropTypes.number,
        headerValue: PropTypes.string,
        canDelete: PropTypes.bool
    }),
    onEdit: PropTypes.func.isRequired,
    onDelete: PropTypes.func.isRequired,
    isInherited: PropTypes.bool
};

export default CustomHeaderCard;
