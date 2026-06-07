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
        <div className='bg-light border border-1 p-3 col-12 col-md-6 col-lg-4'>
            <h3 class="h5 text-dark mb-2">{title}</h3>
            {formattedDate && <span className='text-muted'><small>Published {formattedDate}</small></span>}
            {description && <p className="card-text text-muted small">{description}</p>}
            <a href={url} target='_blank' rel='noopener noreferrer' className='btn btn-outline-primary'>Read more</a>
        </div>
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
