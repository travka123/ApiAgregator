interface Task {
    id: number;
    ownerId: number;
    name: number;
    description: number;
    apiName: string;
    expression: string;
    lastFire: Date|null;
    parameters: any;
}

export default Task;