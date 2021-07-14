import {User} from "./user";
import {Parent} from "./parent";

export class Child extends User{
    ParentId: string;
    Parent?: Parent;
    TimeRestrictions?: [];
  }