import { useContext } from "react";
import { Link } from "react-router-dom";
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

                    <div className="col-3 col-md-2">
                        {account.isAdmin ? 
                            <div>
                                <div>
                                    <Link to='/'><h3>tasks</h3></Link>
                                </div>
                                <div>
                                    <Link to='/admin'><h3>admin</h3></Link>
                                </div>
                            </div> : 
                            null}
                    </div>

                    <div className="col-12 col-md-8" style={{'minWidth': '20em'}}>
                        {children}
                    </div>

                    <div className="col-3 col-md-2">
                        <div>{account.login}</div>
                        <div>{account.email}</div>
                        <button  onClick={() => setAccount(null)}>Logout</button>
                    </div>
                    
                </div>
            </div>
        </div>
    );
}

export default Page;