import PropTypes from 'prop-types';
import { Button, Card } from 'react-bootstrap';

function GuideCard(props) {
    const title = props.guide.title ?? '';
    const description = props.guide.description ?? '';
    const url = props.guide.url ?? '';
    const date = props.guide.date ?? '';

    const formatDate = (value) => {
        if (!value) {
            return '';
        }

        const parsed = new Date(value);
        return Number.isNaN(parsed.getTime()) ? '' : parsed.toLocaleDateString('en-GB');
    };

    const formattedDate = formatDate(date);

    return (
        <Card className='my-3 h-100'>
            <Card.Header className='fw-bold'>{title}</Card.Header>
            <Card.Body>
                {description && <Card.Text>{description}</Card.Text>}
                {formattedDate && <Card.Text className='text-muted'><small>Published {formattedDate}</small></Card.Text>}
            </Card.Body>
            <Card.Footer>
                <Button variant='primary' as='a' href={url} target='_blank' rel='noopener noreferrer'>Read guide</Button>
            </Card.Footer>
        </Card>
    );
}

GuideCard.propTypes = {
    guide: PropTypes.shape({
        title: PropTypes.string,
        url: PropTypes.string,
        description: PropTypes.string,
        date: PropTypes.string
    }).isRequired
};

export default GuideCard;
