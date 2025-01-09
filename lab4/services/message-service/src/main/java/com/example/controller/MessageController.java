package com.example.controller;

import com.example.model.Message;
import com.example.service.MessageService;
import com.example.service.UserServiceClient;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@Slf4j
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
        log.info("Received request to post a message. Username: {}, Content: {}", username, content);

        if (!userServiceClient.userExists(username)) {
            log.warn("User not found: {}", username);
            return ResponseEntity.status(HttpStatus.NOT_FOUND).body("User not found");
        }

        Message message = new Message(messageService.getMessagesCount().toString(), content, username);
        messageService.addMessage(message);
        log.info("Message posted successfully by user: {}. Message ID: {}", username, message.getId());
        return ResponseEntity.status(HttpStatus.CREATED).body("Message posted successfully");
    }

    @GetMapping("/{id}")
    public ResponseEntity<Boolean> getMessage(@PathVariable("id") String id) {
        log.info("Received request to check message existence. Message ID: {}", id);

        if (messageService.check(id)) {
            log.info("Message found. Message ID: {}", id);
            return ResponseEntity.ok(Boolean.TRUE);
        }

        log.warn("Message not found. Message ID: {}", id);
        return ResponseEntity.status(HttpStatus.NOT_FOUND).body(Boolean.FALSE);
    }

    @GetMapping("/getAll")
    public ResponseEntity<List<Message>> getAllMessages() {
        log.info("Received request to fetch all messages");

        List<Message> messages = messageService.getAllMessages();
        if (messages.isEmpty()) {
            log.warn("No messages found");
            return ResponseEntity.status(HttpStatus.NO_CONTENT).body(messages);
        }

        log.info("Successfully retrieved all messages. Total count: {}", messages.size());
        return ResponseEntity.ok(messages);
    }
}
