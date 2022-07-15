import { useContext } from "react";
import { AccountContext, SetAccount, SetAccountContext } from "../components/Authentication";
import Account from "../types/Account";

interface PageProps {
    children: React.ReactNode;
}

const Page: React.FC<PageProps> = ({children} : PageProps) => {

    const account = useContext<Account>(AccountContext);
    const setAccount = useContext<SetAccount>(SetAccountContext);

    return(
        <div className="Page">
            <div className="container mt-4">
                <div className="row">

                    <div className="col">
                        
                    </div>

                    <div className="col-6">
                        {children}
                    </div>

                    <div className="col">
                        <div>Login: {account.login}</div>
                        <div>Email: {account.email}</div>
                        <button  onClick={() => setAccount(null)}>Logout</button>
                    </div>
                    
                </div>
            </div>
        </div>
    );
}

export default Page;