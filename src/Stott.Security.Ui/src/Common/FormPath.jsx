import { Form, Button } from 'react-bootstrap';
import PropTypes from 'prop-types';

function FormPath(props) {

    const handleOnBlur = (event) => {
        var sanitizedValue = event.target.value.trim();
        if (sanitizedValue.startsWith('http://') || sanitizedValue.startsWith('https://')) {
            const url = new URL(sanitizedValue);
            sanitizedValue = url.pathname;
        }

        if (!sanitizedValue.startsWith('/')) {
            sanitizedValue = '/' + sanitizedValue; // Ensure it starts with a slash
        }

        sanitizedValue = sanitizedValue.replace(/[^a-zA-Z0-9/\-_.]/g, ''); // Remove invalid characters
        props.onChange(sanitizedValue);
    };

    return (
        <Form.Group className="input-group mb-3">
            <Form.Control type="text" placeholder="Enter path" value={props.value} onChange={(e) => props.onChange(e.target.value)} onBlur={handleOnBlur} />
            <Button variant="danger" className="input-group-append" aria-label="Remove path" onClick={props.onRemove}>X</Button>
        </Form.Group>
    );
}

export default FormPath;

FormPath.propTypes = {
    value: PropTypes.string.isRequired,
    onChange: PropTypes.func.isRequired,
    onRemove: PropTypes.func.isRequired
};
