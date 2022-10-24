import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Container, Col, Form, Row } from 'react-bootstrap';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import Moment from "react-moment";

function AuditContainer() {

    const [mounted, setMounted] = useState(false);
    const [auditUsers, setAuditUsers] = useState([])
    const [auditHistory, setAuditHistory] = useState([])

    const [startDate, setStartDate] = useState(new Date());
    const [endDate, setEndDate] = useState(new Date());

    const [selectedUser, setSelectedUser] = useState('')
    const [selectedOperationType, setSelectedOperationType] = useState('')
    const [selectedRecordType, setSelectedRecordType] = useState('')

    const setMonthStart = () => {
        var today = new Date();
        setStartDate(new Date(today.getFullYear(), today.getMonth(), today.getDate() - 7));
    }

    const handleSelectUser = (event) => {
        setSelectedUser(event.target.value);
    }

    const handleSelectOperationType = (event) => {
        setSelectedOperationType(event.target.value);
    }

    const handleSelectRecordType = (event) => {
        setSelectedRecordType(event.target.value);
    }

    const getAuditHistory = async () => {
        const response = await axios.get(process.env.REACT_APP_AUDIT_LIST_URL, {params: {
            from: startDate,
            to: endDate,
            actionedBy: selectedUser,
            recordType: selectedRecordType,
            operationType: selectedOperationType
        }});
        setAuditHistory(response.data);
    }

    const getAuditUsers = async () => {
        const response = await axios.get(process.env.REACT_APP_AUDIT_USER_URL);
        setAuditUsers(response.data);
    }

    const renderAuditHistory = () => {
        return auditHistory && auditHistory.map((auditEntry, index) => {
            const { id, actioned, actionedBy, operationType, recordType, field, oldValue, newValue } = auditEntry
            return (
                <tr key={id}>
                    <td>{recordType}</td>
                    <td>{field}</td>
                    <td>{operationType}</td>
                    <td><Moment format="YYYY-MM-DD HH:mm:ss">{actioned}</Moment></td>
                    <td>{actionedBy}</td>
                    <td className='allow-word-break'>{oldValue}</td>
                    <td className='allow-word-break'>{newValue}</td>
                </tr>
            )
        })
    }

    useEffect(() => {
        if (!mounted){
            getAuditUsers();
            setMonthStart();
            setMounted(true);
        }
        else{
            getAuditHistory();
        }
    }, [mounted, startDate, endDate, selectedUser, selectedOperationType, selectedRecordType])

    return (
        <>
            <Container>
                <Row className="mb-3">
                    <Col xs={12} sm={6} lg={4}>
                        <Form.Group>
                            <Form.Label id='lblSelectUser'>User</Form.Label>
                            <Form.Select value={selectedUser} onChange={handleSelectUser} aria-describedby='lblSelectUser' className='form-control'>
                                <option value=''>All</option>
                                {auditUsers.map(user => (
                                    <option key={user} value={user}>{user}</option>
                                ))}
                            </Form.Select>
                        </Form.Group>
                    </Col>
                    <Col xs={12} sm={6} lg={4}>
                        <Form.Group>
                            <Form.Label id='lblSelectOperationType'>Operation</Form.Label>
                            <Form.Select value={selectedOperationType} onChange={handleSelectOperationType} aria-describedby='lblSelectOperationType' className='form-control'>
                                <option value=''>All</option>
                                <option value='Added'>Added</option>
                                <option value='Deleted'>Deleted</option>
                                <option value='Modified'>Modified</option>
                            </Form.Select>
                        </Form.Group>
                    </Col>
                    <Col xs={12} sm={6} lg={4}>
                        <Form.Group>
                            <Form.Label id='lblSelectRecordType'>Record</Form.Label>
                            <Form.Select value={selectedRecordType} onChange={handleSelectRecordType} aria-describedby='lblSelectRecordType' className='form-control'>
                                <option value=''>All</option>
                                <option value='CSP Settings'>CSP Settings</option>
                                <option value='CSP Source'>CSP Source</option>
                                <option value='Security Header Settings'>Security Header Settings</option>
                            </Form.Select>
                        </Form.Group>
                    </Col>
                </Row>
                <Row className="mb-3">
                    <Col xs={12} sm={6} lg={6}>
                        <Form.Group>
                            <Form.Label id='lblSelectDateFrom'>From Date</Form.Label>
                            <DatePicker selected={startDate} onChange={(date) => setStartDate(date)} className='form-control' ariaDescribedBy='lblSelectDateFrom' />
                        </Form.Group>
                    </Col>
                    <Col xs={12} sm={6} lg={6}>
                        <Form.Group>
                            <Form.Label id='lblSelectDateTo'>To Date</Form.Label>
                            <DatePicker selected={endDate} onChange={(date) => setEndDate(date)} className='form-control' ariaDescribedBy='lblSelectDateTo' />
                        </Form.Group>
                    </Col>
                </Row>
            </Container>
            <Container>
                <table className='table table-striped'>
                    <thead>
                        <tr>
                            <th>Data Type</th>
                            <th>Property</th>
                            <th>Operation Type</th>
                            <th>Actioned</th>
                            <th>Actioned By</th>
                            <th>Old Value</th>
                            <th>New Value</th>
                        </tr>
                    </thead>
                    <tbody>
                        {renderAuditHistory()}
                    </tbody>
                </table>
            </Container>
        </>
    )
}

export default AuditContainer