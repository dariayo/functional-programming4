package com.example.controller;

import com.example.model.Message;
import com.example.service.MessageService;
import com.example.service.UserServiceClient;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

@RestController
@RequestMapping("/messages")
public class MessageController {
    private final MessageService messageService;
    private final UserServiceClient userServiceClient;

    public MessageController(MessageService messageService, UserServiceClient userServiceClient) {
        this.userServiceClient = userServiceClient;
        this.messageService = messageService;
    }

    @PostMapping("/post")
    public ResponseEntity<String> postMessage(@RequestParam("username") String username, @RequestParam("content") String content) {
        if (!userServiceClient.userExists(username)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).body("User not found");
        }

        Message message = new Message(messageService.getMessagesCount().toString(), content, username);
        messageService.addMessage(message);
        return ResponseEntity.status(HttpStatus.CREATED).body("Message posted successfully");
    }

    @GetMapping("/{id}")
    public ResponseEntity<Boolean> getMessage(@PathVariable("id") String id) {
        if (messageService.check(id))
            return ResponseEntity.ok(Boolean.TRUE);
        return ResponseEntity.status(HttpStatus.NOT_FOUND).body(Boolean.FALSE);
    }

    @GetMapping("/getAll")
    public ResponseEntity<List<Message>> getAllMessages() {
        List<Message> messages = messageService.getAllMessages();
        return ResponseEntity.ok(messages);
    }
}