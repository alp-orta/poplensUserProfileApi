﻿namespace poplensUserProfileApi.Models.Dtos {
    public class CreateCommentRequest {
        public string Content { get; set; }
        public Guid? ParentCommentId { get; set; }
    }
}