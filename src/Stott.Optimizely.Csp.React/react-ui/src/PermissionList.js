import React, { Component } from "react";

class PermissionList extends Component {
    constructor(props) {
        super(props)
        this.state = {
            cspSources: [
                { id: 1, name: "https://*.example.com/", directives: "default-src,script-src,style-src" },
                { id: 2, name: "https://*.microsoft.com/", directives: "default-src,script-src,style-src" }
            ]
        }
    }

    renderPermissionList() {
        return this.state.cspSources.map((cspSource, index) => {
            const { id, name, directives } = cspSource
            return (
                <tr key={id}>
                    <td>{name}</td>
                    <td>{directives}</td>
                    <td>
                        hello world
                    </td>
                </tr>
            )
        })
    }

    render(){
        return(
            <div>
                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>Source</th>
                            <th>Directives</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.renderPermissionList()}
                    </tbody>
                </table>
            </div>
        )
    }
}

export default PermissionList;