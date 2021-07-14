import { User } from "./user.model";

export class Friendship {
    UserId: string;
    User: User;
    FriendId?: string;
    Friend?: User;
    isBlocked?: boolean;
}