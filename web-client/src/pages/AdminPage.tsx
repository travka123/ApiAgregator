import React, { useContext, useEffect, useState } from "react"
import { getUserStats } from "../Api";
import { AccountContext } from "../components/Authentication";
import UserStatCard from "../components/UserStatCard";
import Account from "../types/Account";
import { UserStat } from "../types/UserStat";

const AdminPage: React.FC = () => {

    const account = useContext<Account>(AccountContext);

    const [stats, setStats] = useState<UserStat[]>([]);

    useEffect(() => {
        if (account.isAdmin) {
            (async () => {
                const result = await getUserStats(account.jwt);
                if (result.success) {
                    setStats(result.data);
                }
            })();
        }
    }, []);

    return (
        <div className="AdminPage">
            {stats.map((us) => <UserStatCard key={us.username} userStat={us} />)}
        </div>
    );
}

export default AdminPage;