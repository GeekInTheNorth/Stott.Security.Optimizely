import { useState } from 'react';
import PropTypes from 'prop-types';
import { Field, Grid, Input } from '@optiaxiom/react';
import FormSelect from '../Common/FormSelect';

const directiveOptions = [
    { label: 'Any Directive', value: '' },
    { label: 'base-uri', value: 'base-uri' },
    { label: 'default-src', value: 'default-src' },
    { label: 'child-src', value: 'child-src' },
    { label: 'frame-src', value: 'frame-src' },
    { label: 'frame-ancestors', value: 'frame-ancestors' },
    { label: 'connect-src', value: 'connect-src' },
    { label: 'form-action', value: 'form-action' },
    { label: 'font-src', value: 'font-src' },
    { label: 'img-src', value: 'img-src' },
    { label: 'media-src', value: 'media-src' },
    { label: 'object-src', value: 'object-src' },
    { label: 'manifest-src', value: 'manifest-src' },
    { label: 'script-src', value: 'script-src' },
    { label: 'script-src-elem', value: 'script-src-elem' },
    { label: 'script-src-attr', value: 'script-src-attr' },
    { label: 'worker-src', value: 'worker-src' },
    { label: 'style-src', value: 'style-src' },
    { label: 'style-src-elem', value: 'style-src-elem' },
    { label: 'style-src-attr', value: 'style-src-attr' }
];

const SourceFilter = (props) => {

    const [sourceFilter, setSourceFilter] = useState("")
    const [directiveFilter, setDirectiveFilter] = useState("")

    const handleSourceFilter = (event) => {
        setSourceFilter(event.target.value);
        props.onSourceFilterUpdate && props.onSourceFilterUpdate(event.target.value, directiveFilter);
    }

    const handleDirectiveFilter = (value) => {
        setDirectiveFilter(value);
        props.onSourceFilterUpdate && props.onSourceFilterUpdate(sourceFilter, value);
    }

    return(
        <Grid gridTemplateColumns={{ base: '1', md: '2' }} gap='16'>
            <Field label='Source'>
                <Input id='txtSourceFilter' type='text' value={sourceFilter} onChange={handleSourceFilter} placeholder='Type a partial url' />
            </Field>
            <Field label='Directive'>
                <FormSelect value={directiveFilter} onValueChange={handleDirectiveFilter} options={directiveOptions} />
            </Field>
        </Grid>
    )
}

SourceFilter.propTypes = {
    onSourceFilterUpdate: PropTypes.func
};

export default SourceFilter;
