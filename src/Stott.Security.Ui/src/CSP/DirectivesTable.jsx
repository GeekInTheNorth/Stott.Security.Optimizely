import PropTypes from 'prop-types';

const directiveDescriptions = [
    { directive: 'base-uri',        description: 'Allows this source to be used within the base element for this site.' },
    { directive: 'default-src',     description: 'Allows this source by default unless one or more sources are defined for a specific permission.' },
    { directive: 'child-src',       description: 'Can contain this source in an iframe or use web workers it provides.' },
    { directive: 'frame-src',       description: 'Can contain this source in an iframe on this site.' },
    { directive: 'frame-ancestors', description: 'This source can contain this site in an iframe.' },
    { directive: 'connect-src',     description: 'Allows links and data requests to this source.' },
    { directive: 'form-action',     description: 'Can use this source within a form action.' },
    { directive: 'font-src',        description: 'Can use fonts from this source.' },
    { directive: 'img-src',         description: 'Can use images from this source.' },
    { directive: 'media-src',       description: 'Can use audio and video files from this source.' },
    { directive: 'object-src',      description: 'Allows content from this source to be used in applet, embed and object elements.' },
    { directive: 'manifest-src',    description: 'Allows this source to be provide a manifest for this site.' },
    { directive: 'script-src',      description: 'Can use javascript from this source.' },
    { directive: 'script-src-elem', description: 'Can use javascript from this source to be used within a script tag.' },
    { directive: 'script-src-attr', description: 'Can use javascript from this source to be used within inline javascript events.' },
    { directive: 'worker-src',      description: 'Can use Worker, SharedWorker and ServiceWorker scripts from this source.' },
    { directive: 'style-src',       description: 'Can use styles from this source.' },
    { directive: 'style-src-elem',  description: 'Can use styles from this source within a style tag.' },
    { directive: 'style-src-attr',  description: 'Can use styles from this source within inline elements.' },
];

function DirectivesTable({ directives }) {
    const active = directives ? directives.split(',') : [];
    const rows = directiveDescriptions.filter(({ directive }) => active.indexOf(directive) >= 0);

    return (
        <table className="table-permissions">
            <tbody>
                {rows.map(({ directive, description }) => (
                    <tr key={directive}>
                        <td className="directive text-nowrap align-top fw-bold">{directive}</td>
                        <td className="directive-description">{description}</td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
}

DirectivesTable.propTypes = {
    directives: PropTypes.string
};

export default DirectivesTable;
