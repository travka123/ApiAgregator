import React, { useContext, useEffect, useState } from "react"
import { addTask, deleteTask, getApis, getTasks, updateTask } from "../Api";
import { AccountContext } from "../components/Authentication";
import ModalWrap from "../components/ModalWrap";
import ParametersCard from "../components/ParametersCard";
import StTaskCard from "../components/StTaskCard";
import TaskForm from "../components/TaskForm";
import Account from "../types/Account";
import Task from "../types/Task";

const TaskPage: React.FC = () => {

    const account = useContext<Account>(AccountContext);

    const [tasks, setTasks] = useState<Task[]>([]);

    console.log(tasks);

    const [apis, setApis] = useState<string[]>([]);

    console.log(apis);

    useEffect(() => {
        (async () => {
            const response = await getTasks(account.jwt);
            if (response.success) {
                setTasks(response.data);
            }
        })();
    }, [account.jwt]);

    useEffect(() => {
        (async () => {
            const response = await getApis(account.jwt);
            if (response.success) {
                setApis(response.data);
            }
        })();
    }, [account.jwt]);

    const onUpdateTask = async (task: Task) => {
        const response = await updateTask(account.jwt, task);
        if (response.success) {
            setTasks([...tasks.filter(t => t.id !== task.id), task]);
        } else if (response.message) {
            return response.message;
        } else {
            return "invalid values";
        }
    }

    const onAddTask = async (task: Task) => {
        const response = await addTask(account.jwt, task);
        if (response.success) {
            setTasks([...tasks, response.data]);
        } else if (response.message) {
            return response.message;
        } else {
            return "invalid values";
        }
    }

    const onTaskDelete = async (id: number) => {
        const response = await deleteTask(account.jwt, id);
        if (response.success) {
            setTasks(tasks.filter(t => t.id !== id));
        }
    }

    return (
        <div className="TaskPage">

            <ModalWrap openBtn={<a href="/" className="card-link"><h3>New</h3></a>} content={<TaskForm apis={apis}
                title='Edit' submitBtnText="Edit" onSubmit={onAddTask} />} />

            {tasks.map(t => <StTaskCard task={t} key={t.id} extra={[
                <ModalWrap openBtn={<a href="/" className="card-link">Parameters</a>} content={<ParametersCard parameters={t.parameters} />} />,
                <ModalWrap openBtn={<a href="/" className="card-link">Edit</a>} content={<TaskForm apis={apis}
                    title='Edit' submitBtnText="Edit" task={t} onSubmit={onUpdateTask} />} />,
                <a href="/" className="card-link me-2" onClick={(e) => {e.preventDefault(); onTaskDelete(t.id)}}>Delete</a>
            ]} />)}
        </div>
    );
}

export default TaskPage;