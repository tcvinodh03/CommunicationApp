export interface Message {
    id: number;
    senderId: number;
    senderUserName: string;
    senderPhotoUrl: string;
    recipientId: number;
    recipientUserName: string;
    recipientrPhotoUrl: string;
    content: string;
    dataRead?: Date;
    messageSent: string;
  }