import React, {Component} from "react";

class EditPermission extends Component {
    constructor(props){
        super(props)
        this.state = {
            id: props.id,
            source: props.source,
            directives: props.directives
        }

        this.handleClick = this.showModal.bind(this)
    }

    showModal() {
        // todo
        console.log(this.state.source)
    }

    render() {
        return (
            <button className="btn btn-primary" type="button" onClick={this.handleClick}>Edit</button>
        )
    }
}

export default EditPermission;