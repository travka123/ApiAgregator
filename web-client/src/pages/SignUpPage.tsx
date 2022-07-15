import React, { useContext, useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router-dom";
import { requestEmailConfirmation, signinByUsername, signup } from "../Api";
import { SetAccount, SetAccountContext } from "../components/Authentication";

interface FormData {
    login: string;
    email: string;
    password: string;
}

const SignUnPage: React.FC = () => {

    const { register, handleSubmit, setError, formState: {errors} } = useForm<FormData>();

    console.log(errors);

    const setAccount = useContext<SetAccount>(SetAccountContext);

    const navigate = useNavigate();

    const submitHandler = (form: FormData) => {
        (async () => {
            const result = await signup(form.login, form.email, form.password);
            if (result.success) {
                const result = await signinByUsername(form.login, form.password);
                if (result.success) {
                    requestEmailConfirmation(result.data.jwt);
                    setAccount(result.data);
                    navigate("/");
                }
            } else if (!result.success && result.message === "username already in use") {
                setError("login", {type: "login already in use"});
            }
            //const result = await signinByUsername(form.login, form.password);
            
        })();
    }

    return (
        <div className="SignInPage">
            <div className="container mt-5" style={{maxWidth: "25em"}}>
                <form onSubmit={handleSubmit(submitHandler)}>
                    <div className="mb-3">
                        <label className="form-label">Login</label>
                        <input {...register("login", {required: true, minLength: 5, maxLength: 100})} className={`form-control ${errors.login !== undefined ? "is-invalid" : ""}`} />
                        {errors.login?.type === "minLength" ? <div className="invalid-feedback">{"login length must be >= 5"}</div> : null}
                        {errors.login?.type === "maxLength" ? <div className="invalid-feedback">{"login length must be <= 100"}</div> : null}
                        {errors.login?.type === "login already in use" ? <div className="invalid-feedback">login already in use</div> : null}
                    </div>
                    <div className="mb-3">
                        <label className="form-label">Email</label>
                        <input {...register("email", {required: true})} type="email" className={`form-control ${errors.email !== undefined ? "is-invalid" : ""}`} />
                    </div>
                    <div className="mb-3">
                        <label className="form-label">Password</label>
                        <input {...register("password", {required: true})} type="password" className={`form-control ${errors.password !== undefined ? "is-invalid" : ""}`} />
                    </div>
                    <div className="col-12">
                        <button className="btn btn-primary me-2" type="submit">sign up</button>
                        <Link className="btn btn-secondary" to='/signin'>sign in</Link>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default SignUnPage;