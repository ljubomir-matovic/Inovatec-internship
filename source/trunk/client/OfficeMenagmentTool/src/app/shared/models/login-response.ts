import { User } from "./user.model"

export interface LoginResponse{
    userData: User
    token: string
}