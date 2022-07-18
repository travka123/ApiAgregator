import React from "react";
import { UserStat } from "../types/UserStat";

interface UserStatCardProps {
    userStat: UserStat;
}

const UserStatCard: React.FC<UserStatCardProps> = ({ userStat }: UserStatCardProps) => {
    return (
        <div className="UserStatCard">
            <div className="card mb-2">
                <div className="card-body">
                    <h5 className="card-title">{userStat.username}</h5>
                    <h6 className="card-subtitle mb-2 text-muted">{userStat.email}</h6>
                    <p className="card-text mb-1">Active tasks: {userStat.totalTasks}</p>
                    <p className="card-text mb-1">Active tasks calls: {userStat.totalCalls}</p>
                    <p className="card-text mb-1">Active tasks failed calls: {userStat.totalErrorCalls}</p>
                </div>
            </div>
        </div>
    );
}

export default UserStatCard;