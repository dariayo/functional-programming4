package com.example.controller;

import com.example.model.User;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.HashMap;
import java.util.Map;

@Slf4j
@RestController
@RequestMapping("/users")
public class UserController {

    private final Map<String, User> userStorage = new HashMap<>();

    @PostMapping("/register")
    public ResponseEntity<String> registerUser(@RequestParam("username") String username) {
        log.info("Received request to register user with username: {}", username);

        if (userStorage.containsKey(username)) {
            log.warn("Attempt to register already existing user: {}", username);
            return ResponseEntity.status(HttpStatus.CONFLICT).body("User already exists");
        }

        userStorage.put(username, new User(username));
        log.info("User registered successfully: {}", username);
        return ResponseEntity.status(HttpStatus.CREATED).body("User registered successfully");
    }

    @GetMapping("/{username}")
    public ResponseEntity<User> getUser(@PathVariable String username) {
        log.info("Received request to fetch user with username: {}", username);

        User user = userStorage.get(username);
        if (user == null) {
            log.warn("User not found: {}", username);
            return ResponseEntity.status(HttpStatus.NOT_FOUND).body(null);
        }

        log.info("User retrieved successfully: {}", username);
        return ResponseEntity.ok(user);
    }

    @GetMapping("/getAll")
    public ResponseEntity<Map<String, User>> getAllUser() {
        log.info("Received request to fetch all users");

        if (userStorage.isEmpty()) {
            log.warn("No users found in storage");
            return ResponseEntity.status(HttpStatus.NO_CONTENT).body(null);
        }

        log.info("All users retrieved successfully. Total count: {}", userStorage.size());
        return ResponseEntity.ok(userStorage);
    }
}
