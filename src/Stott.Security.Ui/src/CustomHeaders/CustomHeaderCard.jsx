import PropTypes from 'prop-types';
import { Button, Card } from 'react-bootstrap';

function CustomHeaderCard(props) {
    const headerName = props.header.headerName ?? '';
    const description = props.header.description ?? '';
    const behavior = props.header.behavior ?? 0;
    const headerValue = props.header.headerValue ?? '';
    const canDelete = props.header.canDelete ?? true;

    const getBehaviorLabel = (behavior) => {
        switch (behavior) {
            case 0: return 'Disabled';
            case 1: return 'Add';
            case 2: return 'Remove';
            default: return 'Unknown';
        }
    };

    return (
        <Card className='my-3'>
            <Card.Header className='fw-bold'>{headerName}</Card.Header>
            <Card.Body>
                {description && <Card.Text><em>{description}</em></Card.Text>}
                <Card.Text><strong>Behaviour:</strong> {getBehaviorLabel(behavior)}</Card.Text>
                {behavior === 1 && (<Card.Text><strong>Value:</strong> {headerValue}</Card.Text>)}
            </Card.Body>
            <Card.Footer>
                <Button variant='primary' onClick={() => props.onEdit(props.header)} className='text-nowrap me-2'>Edit</Button>
                {canDelete && (<Button variant='danger' onClick={() => props.onDelete(props.header.id, props.header.headerName)} className='text-nowrap'>Delete</Button>)}
            </Card.Footer>
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
    onDelete: PropTypes.func.isRequired
};

export default CustomHeaderCard;
