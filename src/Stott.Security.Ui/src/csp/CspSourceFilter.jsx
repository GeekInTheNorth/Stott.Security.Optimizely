import { useState } from 'react';
import { Form, InputGroup } from 'react-bootstrap';
import PropTypes from 'prop-types';

const CspSourceFilter = (props) => {

    const [sourceFilter, setSourceFilter] = useState("")
    const [directiveFilter, setDirectiveFilter] = useState("")

    const handleSourceFilter = (event) => {
        setSourceFilter(event.target.value);
        props.onSourceFilterUpdate && props.onSourceFilterUpdate(event.target.value, directiveFilter);
    }

    const handleDirectiveFilter = (event) => {
        setDirectiveFilter(event.target.value);
        props.onSourceFilterUpdate && props.onSourceFilterUpdate(sourceFilter, event.target.value);
    }

    return(
        <InputGroup>
            <InputGroup.Text id='lblSourceFilters'>Filter</InputGroup.Text>
            <Form.Control id='txtSourceFilter' type='text' value={sourceFilter} onChange={handleSourceFilter} aria-describedby='lblSourceFilters' placeholder='Type a partial url'></Form.Control>
            <Form.Select value={directiveFilter} onChange={handleDirectiveFilter} aria-describedby='lblSourceFilters' className='form-control'>
                <option value=''>Any Directive</option>
                <option value='base-uri'>base-uri</option>
                <option value='default-src'>default-src</option>
                <option value='child-src'>child-src</option>
                <option value='frame-src'>frame-src</option>
                <option value='frame-ancestors'>frame-ancestors</option>
                <option value='connect-src'>connect-src</option>
                <option value='form-action'>form-action</option>
                <option value='font-src'>font-src</option>
                <option value='img-src'>img-src</option>
                <option value='media-src'>media-src</option>
                <option value='object-src'>object-src</option>
                <option value='manifest-src'>manifest-src</option>
                <option value='script-src'>script-src</option>
                <option value='script-src-elem'>script-src-elem</option>
                <option value='script-src-attr'>script-src-attr</option>
                <option value='worker-src'>worker-src</option>
                <option value='style-src'>style-src</option>
                <option value='style-src-elem'>style-src-elem</option>
                <option value='style-src-attr'>style-src-attr</option>
            </Form.Select>
        </InputGroup>
    )
}

CspSourceFilter.propTypes = {
  onSourceFilterUpdate: PropTypes.func.isRequired
};

export default CspSourceFilter;