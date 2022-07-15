import React, { useContext, useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router-dom";
import { signinByUsername } from "../Api";
import { SetAccount, SetAccountContext } from "../components/Authentication";

interface FormData {
    login: string;
    password: string;
}

const SignInPage: React.FC = () => {

    const { register, handleSubmit } = useForm<FormData>();

    const [failed, setFailed] = useState<boolean>(false);

    const setAccount = useContext<SetAccount>(SetAccountContext);

    const navigate = useNavigate();

    const submitHandler = (form: FormData) => {
        (async () => {
            const result = await signinByUsername(form.login, form.password);
            if (result.success) {
                setAccount(result.data);
                navigate("/", { replace: true });
            }
            else {
                setFailed(true);
            }
        })();
    }

    return (
        <div className="SignInPage">
            <div className="container mt-5" style={{ maxWidth: "25em" }}>
                <form onSubmit={handleSubmit(submitHandler)}>
                    <div className="mb-3">
                        <label className="form-label">Login</label>
                        <input {...register("login")} className="form-control" />
                    </div>
                    <div className="mb-3">
                        <label className="form-label">Password</label>
                        <input {...register("password")} type="password" className="form-control" />
                    </div>
                    <div className="mb-3">
                        <button className="btn btn-primary me-2" type="submit">sign in</button>
                        <Link className="btn btn-secondary" to='/signup'>sign up</Link>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default SignInPage;